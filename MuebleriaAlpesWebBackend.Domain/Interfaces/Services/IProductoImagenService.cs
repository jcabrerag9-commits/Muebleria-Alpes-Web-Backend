using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IProductoImagenService
    {
        Task<ApiResponse<int>> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden);
        Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId);
        Task<ApiResponse<bool>> EliminarImagenAsync(int imagenId);
        Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId);
        Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId);
    }
}
