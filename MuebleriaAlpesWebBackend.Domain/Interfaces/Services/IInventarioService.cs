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
        Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request);
    }

    public interface IInventarioService
    {
        Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request);
        Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request);
        Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request);
        Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId);
        Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId);
    }
}
