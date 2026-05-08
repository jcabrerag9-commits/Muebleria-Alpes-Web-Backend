using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IProductoRepository
    {
        Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request, IDbTransaction? transaction = null);
        Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, object request, IDbTransaction? transaction = null);
        Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request, IDbTransaction? transaction = null);
        Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId);
        Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync();
    }

    public interface IInventarioRepository
    {
        Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null);
        Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null);
        Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request, IDbTransaction? transaction = null);
        Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, IDbTransaction? transaction = null);
        Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId);
        Task<InventarioResponse<bool>> RegistrarAjusteAsync(AjusteStockRequest request, IDbTransaction? transaction = null);
    }
}
