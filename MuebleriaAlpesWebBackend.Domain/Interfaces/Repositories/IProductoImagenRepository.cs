using MuebleriaAlpesWebBackend.Domain.Models;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IProductoImagenRepository
    {
        Task<int> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden, IDbTransaction? transaction = null);
        Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId);
        Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId);
        Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId);
        Task<bool> EliminarImagenAsync(int imagenId, IDbTransaction? transaction = null);
    }
}
