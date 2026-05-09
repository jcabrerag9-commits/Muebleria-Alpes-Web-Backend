using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IPrecioRepository
    {
        Task<int> CreateAsync(PrecioProducto precio);
        Task UpdateAsync(ActualizarPrecioRequest request);
        Task<decimal> GetPrecioVigenteAsync(int productoId, int monedaId);
        Task<decimal> GetPrecioFinalAsync(int productoId, int monedaId);
        Task<IEnumerable<PrecioProducto>> GetHistorialByProductoAsync(int productoId);
    }
}
