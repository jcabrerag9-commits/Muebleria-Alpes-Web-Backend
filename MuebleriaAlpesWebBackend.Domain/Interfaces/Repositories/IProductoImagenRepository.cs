using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IProductoImagenRepository
    {
        Task<int> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden, IDbTransaction? transaction = null, CancellationToken ct = default);
        Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId, CancellationToken ct = default);
        Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId, CancellationToken ct = default);
        Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId, CancellationToken ct = default);
        Task<bool> EliminarImagenAsync(int imagenId, IDbTransaction? transaction = null, CancellationToken ct = default);
    }
}
