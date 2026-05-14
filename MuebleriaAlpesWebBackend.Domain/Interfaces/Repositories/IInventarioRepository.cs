using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IProductoRepository
    {
        Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, ActualizarProductoRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId, CancellationToken ct = default);
        Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync(CancellationToken ct = default);
    }

    public interface IInventarioRepository
    {
        Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, int? usuarioId = null, string? observacion = null, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId, CancellationToken ct = default);
        Task<InventarioResponse<bool>> RegistrarAjusteAsync(AjusteStockRequest request, IDbTransaction? transaction = null, CancellationToken ct = default);
        
        Task<IEnumerable<ReservaDTO>> ObtenerReservasPorProductoAsync(int productoId, CancellationToken ct = default);
        Task<ReservaDTO?> ObtenerReservaPorIdAsync(int reservaId, CancellationToken ct = default);

        Task<IEnumerable<KardexDTO>> ObtenerKardexPorProductoAsync(int productoId, CancellationToken ct = default);
        Task<IEnumerable<KardexDTO>> ObtenerMovimientosGlobalesAsync(MovimientoFiltroRequest filtro, CancellationToken ct = default);
    }
}
