using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repo;

        public BannerService(IBannerRepository repo)
        {
            _repo = repo;
        }

        // ── Consultas ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<BannerResponseDto>> GetAllAsync(string? estado = null, string? posicion = null)
        {
            var lista = await _repo.GetAllAsync(estado, posicion);
            var dtos   = new List<BannerResponseDto>();

            foreach (var b in lista)
            {
                var clicks = await _repo.GetTotalClicksAsync(b.BanBanner);
                dtos.Add(MapToDto(b, clicks));
            }
            return dtos;
        }

        public async Task<BannerResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            var clicks = await _repo.GetTotalClicksAsync(id);
            return MapToDto(entity, clicks);
        }

        public async Task<IEnumerable<BannerResponseDto>> GetVigentesAsync(string? posicion = null)
        {
            var lista = await _repo.GetVigentesAsync(posicion);
            var dtos   = new List<BannerResponseDto>();

            foreach (var b in lista)
            {
                var clicks = await _repo.GetTotalClicksAsync(b.BanBanner);
                dtos.Add(MapToDto(b, clicks));
            }
            return dtos;
        }

        public async Task<BannerEstadisticasDto?> GetEstadisticasAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            var clicks = await _repo.GetClicksByBannerAsync(id);
            var lista  = clicks.ToList();

            return new BannerEstadisticasDto
            {
                BanBanner      = entity.BanBanner,
                BanTitulo      = entity.BanTitulo,
                TotalClicks    = lista.Count,
                ClicksWeb      = lista.Count(c => c.ClbPlataforma == "WEB"),
                ClicksMobile   = lista.Count(c => c.ClbPlataforma == "MOBILE"),
                ClicksApp      = lista.Count(c => c.ClbPlataforma == "APP"),
                ClicksTablet   = lista.Count(c => c.ClbPlataforma == "TABLET"),
                UltimoClick    = lista.Any() ? lista.Max(c => c.ClbFechaClick) : null
            };
        }

        // ── CRUD ──────────────────────────────────────────────────────────────

        public async Task<BannerResponseDto> CreateAsync(BannerCreateDto dto)
        {
            ValidarFechas(dto.BanFechaInicio, dto.BanFechaFin);

            // El estado inicial depende de si la fecha de inicio ya pasó
            var estado = dto.BanFechaInicio <= DateTime.Now
                ? EstadoBanner.Activo
                : EstadoBanner.Programado;

            var entity = new Banner
            {
                BanTitulo      = dto.BanTitulo.Trim(),
                BanImagenUrl   = dto.BanImagenUrl.Trim(),
                BanEnlace      = dto.BanEnlace?.Trim(),
                BanPosicion    = dto.BanPosicion?.ToUpper().Trim(),
                BanFechaInicio = dto.BanFechaInicio,
                BanFechaFin    = dto.BanFechaFin,
                BanEstado      = estado
            };

            var newId = await _repo.CreateAsync(entity);
            return (await GetByIdAsync(newId))!;
        }

        public async Task<BannerResponseDto?> UpdateAsync(long id, BannerUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.BanTitulo))
                entity.BanTitulo = dto.BanTitulo.Trim();

            if (!string.IsNullOrWhiteSpace(dto.BanImagenUrl))
                entity.BanImagenUrl = dto.BanImagenUrl.Trim();

            if (dto.BanEnlace is not null)
                entity.BanEnlace = dto.BanEnlace.Trim();

            if (dto.BanPosicion is not null)
                entity.BanPosicion = dto.BanPosicion.ToUpper().Trim();

            if (dto.BanFechaInicio.HasValue || dto.BanFechaFin.HasValue)
            {
                var inicio = dto.BanFechaInicio ?? entity.BanFechaInicio;
                var fin    = dto.BanFechaFin    ?? entity.BanFechaFin;
                ValidarFechas(inicio, fin);
                entity.BanFechaInicio = inicio;
                entity.BanFechaFin    = fin;
            }

            if (!string.IsNullOrWhiteSpace(dto.BanEstado))
            {
                if (!EstadoBanner.Todos.Contains(dto.BanEstado.ToUpper()))
                    throw new ArgumentException($"Estado inválido: '{dto.BanEstado}'. Válidos: {string.Join(", ", EstadoBanner.Todos)}");
                entity.BanEstado = dto.BanEstado.ToUpper();
            }

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (!await _repo.ExistsAsync(id)) return false;
            return await _repo.DeleteAsync(id);
        }

        // ── Clicks ────────────────────────────────────────────────────────────

        public async Task<ClickBannerResponseDto> RegistrarClickAsync(long bannerId, ClickBannerCreateDto dto)
        {
            if (!await _repo.ExistsAsync(bannerId))
                throw new KeyNotFoundException($"Banner ID {bannerId} no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.ClbPlataforma) &&
                !PlataformaBanner.Todas.Contains(dto.ClbPlataforma.ToUpper()))
                throw new ArgumentException($"Plataforma inválida: '{dto.ClbPlataforma}'. Válidas: {string.Join(", ", PlataformaBanner.Todas)}");

            var click = new ClickBanner
            {
                BanBanner     = bannerId,
                CliCliente    = dto.CliCliente,
                ClbFechaClick = DateTime.Now,
                ClbPlataforma = dto.ClbPlataforma?.ToUpper(),
                ClbOrigen     = dto.ClbOrigen,
                ClbDetalle    = dto.ClbDetalle
            };

            var newId = await _repo.RegistrarClickAsync(click);
            click.ClbClickBanner = newId;

            return new ClickBannerResponseDto
            {
                ClbClickBanner = click.ClbClickBanner,
                BanBanner      = click.BanBanner,
                CliCliente     = click.CliCliente,
                ClbFechaClick  = click.ClbFechaClick,
                ClbPlataforma  = click.ClbPlataforma,
                ClbOrigen      = click.ClbOrigen
            };
        }

        // ── Validaciones ──────────────────────────────────────────────────────

        private static void ValidarFechas(DateTime inicio, DateTime? fin)
        {
            if (fin.HasValue && fin.Value <= inicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        // ── Mapeos ────────────────────────────────────────────────────────────

        private static BannerResponseDto MapToDto(Banner b, int totalClicks) => new()
        {
            BanBanner      = b.BanBanner,
            BanTitulo      = b.BanTitulo,
            BanImagenUrl   = b.BanImagenUrl,
            BanEnlace      = b.BanEnlace,
            BanPosicion    = b.BanPosicion,
            BanFechaInicio = b.BanFechaInicio,
            BanFechaFin    = b.BanFechaFin,
            BanEstado      = b.BanEstado,
            TotalClicks    = totalClicks
        };
    }
}