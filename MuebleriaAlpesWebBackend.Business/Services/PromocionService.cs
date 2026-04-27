using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class PromocionService : IPromocionService
    {
        private readonly IPromocionRepository _repo;

        public PromocionService(IPromocionRepository repo)
        {
            _repo = repo;
        }

        // ── Consultas ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<PromocionListDto>> GetAllAsync(string? estado = null, string? tipo = null)
        {
            var lista = await _repo.GetAllAsync(estado, tipo);
            return lista.Select(MapToListDto);
        }

        public async Task<PromocionResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            var productos = await _repo.GetProductosByPromocionAsync(id);
            return MapToResponseDto(entity, productos);
        }

        public async Task<PromocionResponseDto?> GetByCodigoAsync(string codigo)
        {
            var entity = await _repo.GetByCodigoAsync(codigo);
            if (entity is null) return null;

            var productos = await _repo.GetProductosByPromocionAsync(entity.PrmPromocion);
            return MapToResponseDto(entity, productos);
        }

        public async Task<IEnumerable<PromocionListDto>> GetVigentesAsync()
        {
            var lista = await _repo.GetVigentesAsync();
            return lista.Select(MapToListDto);
        }

        // ── CRUD ──────────────────────────────────────────────────────────────

        public async Task<PromocionResponseDto> CreateAsync(PromocionCreateDto dto)
        {
            ValidarTipo(dto.PrmTipo);
            ValidarFechas(dto.PrmFechaInicio, dto.PrmFechaFin);
            ValidarValor(dto.PrmTipo, dto.PrmValor);

            if (await _repo.CodigoExistsAsync(dto.PrmCodigo))
                throw new InvalidOperationException($"Ya existe una promoción con el código '{dto.PrmCodigo}'.");

            var entity = new Promocion
            {
                PrmCodigo      = dto.PrmCodigo.ToUpper().Trim(),
                PrmNombre      = dto.PrmNombre.Trim(),
                PrmDescripcion = dto.PrmDescripcion?.Trim(),
                PrmTipo        = dto.PrmTipo.ToUpper(),
                PrmValor       = dto.PrmValor,
                PrmFechaInicio = dto.PrmFechaInicio,
                PrmFechaFin    = dto.PrmFechaFin,
                PrmEstado      = EstadoPromocion.Activo
            };

            var newId = await _repo.CreateAsync(entity);

            // Agregar productos si vienen
            if (dto.Productos is { Count: > 0 })
            {
                foreach (var prod in dto.Productos)
                {
                    ValidarProductoOVariante(prod.ProProducto, prod.PvaProductoVariante);
                    await _repo.AddProductoAsync(new PromocionProducto
                    {
                        PrmPromocion        = newId,
                        ProProducto         = prod.ProProducto,
                        PvaProductoVariante = prod.PvaProductoVariante,
                        PpoEstado           = "ACTIVO",
                        PpoFechaCreacion    = DateTime.Now
                    });
                }
            }

            return (await GetByIdAsync(newId))!;
        }

        public async Task<PromocionResponseDto?> UpdateAsync(long id, PromocionUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.PrmNombre))
                entity.PrmNombre = dto.PrmNombre.Trim();

            if (dto.PrmDescripcion is not null)
                entity.PrmDescripcion = dto.PrmDescripcion.Trim();

            if (dto.PrmValor.HasValue)
            {
                ValidarValor(entity.PrmTipo, dto.PrmValor);
                entity.PrmValor = dto.PrmValor;
            }

            if (dto.PrmFechaInicio.HasValue || dto.PrmFechaFin.HasValue)
            {
                var inicio = dto.PrmFechaInicio ?? entity.PrmFechaInicio;
                var fin    = dto.PrmFechaFin    ?? entity.PrmFechaFin;
                ValidarFechas(inicio, fin);
                entity.PrmFechaInicio = inicio;
                entity.PrmFechaFin    = fin;
            }

            if (!string.IsNullOrWhiteSpace(dto.PrmEstado))
            {
                var validos = new[] { "ACTIVO", "INACTIVO", "EXPIRADO" };
                if (!validos.Contains(dto.PrmEstado.ToUpper()))
                    throw new ArgumentException($"Estado inválido: '{dto.PrmEstado}'. Válidos: {string.Join(", ", validos)}");
                entity.PrmEstado = dto.PrmEstado.ToUpper();
            }

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (!await _repo.ExistsAsync(id)) return false;
            return await _repo.DeleteAsync(id);
        }

        // ── Productos ─────────────────────────────────────────────────────────

        public async Task<PromocionProductoResponseDto> AddProductoAsync(long promocionId, PromocionProductoCreateDto dto)
        {
            if (!await _repo.ExistsAsync(promocionId))
                throw new KeyNotFoundException($"Promoción ID {promocionId} no encontrada.");

            ValidarProductoOVariante(dto.ProProducto, dto.PvaProductoVariante);

            var pp = new PromocionProducto
            {
                PrmPromocion        = promocionId,
                ProProducto         = dto.ProProducto,
                PvaProductoVariante = dto.PvaProductoVariante,
                PpoEstado           = "ACTIVO",
                PpoFechaCreacion    = DateTime.Now
            };

            pp.PpoPromocionProducto = await _repo.AddProductoAsync(pp);
            return MapProductoToDto(pp);
        }

        public async Task<bool> RemoveProductoAsync(long promocionId, long ppoId)
        {
            if (!await _repo.ExistsAsync(promocionId))
                throw new KeyNotFoundException($"Promoción ID {promocionId} no encontrada.");
            return await _repo.RemoveProductoAsync(ppoId);
        }

        // ── Validaciones ──────────────────────────────────────────────────────

        private static void ValidarTipo(string tipo)
        {
            if (!TipoPromocion.Todos.Contains(tipo.ToUpper()))
                throw new ArgumentException($"Tipo inválido: '{tipo}'. Válidos: {string.Join(", ", TipoPromocion.Todos)}");
        }

        private static void ValidarFechas(DateTime inicio, DateTime fin)
        {
            if (fin <= inicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        private static void ValidarValor(string tipo, decimal? valor)
        {
            if (tipo.ToUpper() is "ENVIO_GRATIS" or "2X1") return;
            if (!valor.HasValue || valor <= 0)
                throw new ArgumentException($"El tipo '{tipo}' requiere un valor mayor a 0.");
            if (tipo.ToUpper() == "PORCENTAJE" && valor > 100)
                throw new ArgumentException("El porcentaje no puede exceder 100.");
        }

        private static void ValidarProductoOVariante(long? productoId, long? varianteId)
        {
            if (productoId is null && varianteId is null)
                throw new ArgumentException("Debe especificar ProProducto o PvaProductoVariante.");
            if (productoId is not null && varianteId is not null)
                throw new ArgumentException("Solo puede especificar ProProducto O PvaProductoVariante, no ambos.");
        }

        // ── Mapeos ────────────────────────────────────────────────────────────

        private static PromocionResponseDto MapToResponseDto(Promocion e, IEnumerable<PromocionProducto> productos) => new()
        {
            PrmPromocion   = e.PrmPromocion,
            PrmCodigo      = e.PrmCodigo,
            PrmNombre      = e.PrmNombre,
            PrmDescripcion = e.PrmDescripcion,
            PrmTipo        = e.PrmTipo,
            PrmValor       = e.PrmValor,
            PrmFechaInicio = e.PrmFechaInicio,
            PrmFechaFin    = e.PrmFechaFin,
            PrmEstado      = e.PrmEstado,
            Productos      = productos.Select(MapProductoToDto).ToList()
        };

        private static PromocionListDto MapToListDto(Promocion e) => new()
        {
            PrmPromocion   = e.PrmPromocion,
            PrmCodigo      = e.PrmCodigo,
            PrmNombre      = e.PrmNombre,
            PrmTipo        = e.PrmTipo,
            PrmValor       = e.PrmValor,
            PrmFechaInicio = e.PrmFechaInicio,
            PrmFechaFin    = e.PrmFechaFin,
            PrmEstado      = e.PrmEstado
        };

        private static PromocionProductoResponseDto MapProductoToDto(PromocionProducto pp) => new()
        {
            PpoPromocionProducto = pp.PpoPromocionProducto,
            ProProducto          = pp.ProProducto,
            PvaProductoVariante  = pp.PvaProductoVariante,
            PpoEstado            = pp.PpoEstado,
            PpoFechaCreacion     = pp.PpoFechaCreacion
        };
    }
}
