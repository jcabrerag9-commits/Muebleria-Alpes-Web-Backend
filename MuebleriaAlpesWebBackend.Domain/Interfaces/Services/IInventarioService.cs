using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IProductoService
    {
        Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request);
        Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId);
        Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync();
        Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, ActualizarProductoRequest request);
        Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request);
    }

    public interface IInventarioService
    {
        Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request);
        Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request);
        Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request);
        /// <summary>Libera cualquier reserva sin validación de motivo. Solo para uso interno/sistema.</summary>
        Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, int? usuarioId = null, string? observacion = null);
        /// <summary>
        /// Valida las reglas de negocio ERP antes de liberar.
        /// Solo permite: APARTADO_MANUAL, RESERVA_TEMPORAL.
        /// Rechaza con error descriptivo: CARRITO, ORDEN_CONFIRMADA.
        /// </summary>
        Task<InventarioResponse<bool>> ValidarYLiberarReservaAsync(int reservaId);
        Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId);
        // --- NUEVOS ---
        Task<IEnumerable<ReservaDTO>> ObtenerReservasPorProductoAsync(int productoId);
        Task<IEnumerable<BodegaDTO>> ObtenerBodegasAsync();
        Task<IEnumerable<KardexDTO>> ObtenerKardexPorProductoAsync(int productoId);
        Task<IEnumerable<KardexDTO>> ObtenerMovimientosGlobalesAsync(MovimientoFiltroRequest filtro);
    }
}
