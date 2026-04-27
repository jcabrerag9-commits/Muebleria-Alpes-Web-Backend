using MuebleriaAlpesWebBackend.Domain.DTOs.Devoluciones;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class DevolucionService : IDevolucionService
    {
        private readonly IDevolucionRepository _repo;

        // Máquina de estados RMA
        private static readonly Dictionary<string, string[]> _transiciones = new()
        {
            { "SOLICITADA",  ["EN_REVISION", "RECHAZADA"] },
            { "EN_REVISION", ["APROBADA",    "RECHAZADA"] },
            { "APROBADA",    ["COMPLETADA"] },
            { "RECHAZADA",   [] },
            { "COMPLETADA",  [] }
        };

        public DevolucionService(IDevolucionRepository repo)
        {
            _repo = repo;
        }

        // ── Consultas ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<DevolucionListDto>> GetAllAsync(string? estado = null, long? clienteId = null)
        {
            var lista = await _repo.GetAllAsync(estado, clienteId);
            return lista.Select(MapToListDto);
        }

        public async Task<DevolucionResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            var detalles = await _repo.GetDetallesByDevolucionAsync(id);
            return MapToResponseDto(entity, detalles);
        }

        public async Task<DevolucionResponseDto?> GetByRmaAsync(string numeroRma)
        {
            var entity = await _repo.GetByRmaAsync(numeroRma);
            if (entity is null) return null;

            var detalles = await _repo.GetDetallesByDevolucionAsync(entity.DevDevolucion);
            return MapToResponseDto(entity, detalles);
        }

        public async Task<IEnumerable<DevolucionListDto>> GetByOrdenVentaAsync(long ordenVentaId)
        {
            var lista = await _repo.GetByOrdenVentaAsync(ordenVentaId);
            return lista.Select(MapToListDto);
        }

        public async Task<IEnumerable<DevolucionListDto>> GetByClienteAsync(long clienteId)
        {
            var lista = await _repo.GetByClienteAsync(clienteId);
            return lista.Select(MapToListDto);
        }

        // ── CRUD ──────────────────────────────────────────────────────────────

        public async Task<DevolucionResponseDto> CreateAsync(DevolucionCreateDto dto)
        {
            // Validar categoría
            var categoria = await _repo.GetCategoriaByIdAsync(dto.CtdCategoriaTipoDev)
                ?? throw new KeyNotFoundException($"Categoría de devolución ID {dto.CtdCategoriaTipoDev} no encontrada.");

            if (categoria.CtdEstado != "ACTIVO")
                throw new InvalidOperationException($"La categoría '{categoria.CtdNombre}' no está activa.");

            if (!dto.Detalles.Any())
                throw new ArgumentException("Debe incluir al menos un ítem en la devolución.");

            var montoTotal = dto.Detalles.Sum(d => d.DdeMonto);
            var numeroRma  = await _repo.GenerarNumeroRmaAsync();

            var devolucion = new Devolucion
            {
                VenOrdenVenta       = dto.VenOrdenVenta,
                CliCliente          = dto.CliCliente,
                CtdCategoriaTipoDev = dto.CtdCategoriaTipoDev,
                DevNumeroRma        = numeroRma,
                DevMotivo           = dto.DevMotivo.Trim(),
                DevMontoTotal       = montoTotal,
                DevEstado           = EstadoDevolucion.Solicitada,
                DevFechaCreacion    = DateTime.Now
            };

            var newId = await _repo.CreateAsync(devolucion);

            // Insertar detalles
            var detalles = dto.Detalles.Select(d => new DevolucionDetalle
            {
                DevDevolucion        = newId,
                VdeOrdenVentaDetalle = d.VdeOrdenVentaDetalle,
                DdeCantidad          = d.DdeCantidad,
                DdeMonto             = d.DdeMonto,
                DdeEstado            = EstadoDetalleDevolucion.Pendiente,
                DdeFechaCreacion     = DateTime.Now
            });

            await _repo.AddDetallesAsync(detalles);

            return (await GetByIdAsync(newId))!;
        }

        public async Task<DevolucionResponseDto?> CambiarEstadoAsync(long id, DevolucionUpdateEstadoDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            var nuevoEstado = dto.DevEstado.ToUpper();

            if (!_transiciones.TryGetValue(entity.DevEstado, out var permitidos) ||
                !permitidos.Contains(nuevoEstado))
            {
                var posibles = _transiciones.ContainsKey(entity.DevEstado)
                    ? string.Join(", ", _transiciones[entity.DevEstado])
                    : "ninguno";
                throw new InvalidOperationException(
                    $"No se puede cambiar de '{entity.DevEstado}' a '{nuevoEstado}'. " +
                    $"Transiciones permitidas: {posibles}");
            }

            await _repo.UpdateEstadoAsync(id, nuevoEstado);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return false;

            if (entity.DevEstado != "SOLICITADA")
                throw new InvalidOperationException(
                    $"No se puede eliminar una devolución en estado '{entity.DevEstado}'. Solo se permite en estado SOLICITADA.");

            return await _repo.DeleteAsync(id);
        }

        // ── Categorías ────────────────────────────────────────────────────────

        public async Task<IEnumerable<CategoriaDevolucionResponseDto>> GetCategoriasAsync(string? estado = null)
        {
            var lista = await _repo.GetCategoriasAsync(estado);
            return lista.Select(MapCategoriaToDto);
        }

        public async Task<CategoriaDevolucionResponseDto?> GetCategoriaByIdAsync(long id)
        {
            var entity = await _repo.GetCategoriaByIdAsync(id);
            return entity is null ? null : MapCategoriaToDto(entity);
        }

        // ── Mapeos ────────────────────────────────────────────────────────────

        private static DevolucionResponseDto MapToResponseDto(Devolucion e, IEnumerable<DevolucionDetalle> detalles) => new()
        {
            DevDevolucion       = e.DevDevolucion,
            VenOrdenVenta       = e.VenOrdenVenta,
            CliCliente          = e.CliCliente,
            CtdCategoriaTipoDev = e.CtdCategoriaTipoDev,
            NombreCategoria     = e.NombreCategoria,
            DevNumeroRma        = e.DevNumeroRma,
            DevMotivo           = e.DevMotivo,
            DevMontoTotal       = e.DevMontoTotal,
            DevEstado           = e.DevEstado,
            DevFechaCreacion    = e.DevFechaCreacion,
            Detalles            = detalles.Select(d => new DevolucionDetalleResponseDto
            {
                DdeDevolucionDetalle  = d.DdeDevolucionDetalle,
                VdeOrdenVentaDetalle  = d.VdeOrdenVentaDetalle,
                DdeCantidad           = d.DdeCantidad,
                DdeMonto              = d.DdeMonto,
                DdeEstado             = d.DdeEstado,
                DdeFechaCreacion      = d.DdeFechaCreacion
            }).ToList()
        };

        private static DevolucionListDto MapToListDto(Devolucion e) => new()
        {
            DevDevolucion    = e.DevDevolucion,
            DevNumeroRma     = e.DevNumeroRma,
            VenOrdenVenta    = e.VenOrdenVenta,
            CliCliente       = e.CliCliente,
            DevEstado        = e.DevEstado,
            DevMontoTotal    = e.DevMontoTotal,
            DevFechaCreacion = e.DevFechaCreacion,
            NombreCategoria  = e.NombreCategoria
        };

        private static CategoriaDevolucionResponseDto MapCategoriaToDto(CategoriaDevolucion e) => new()
        {
            CtdCategoriaTipoDev = e.CtdCategoriaTipoDev,
            CtdCodigo           = e.CtdCodigo,
            CtdNombre           = e.CtdNombre,
            CtdDescripcion      = e.CtdDescripcion,
            CtdEstado           = e.CtdEstado
        };
    }
}
