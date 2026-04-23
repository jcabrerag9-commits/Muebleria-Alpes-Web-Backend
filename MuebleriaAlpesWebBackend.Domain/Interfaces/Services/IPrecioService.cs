using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IPrecioService
    {
        Task<int> RegistrarPrecioAsync(PrecioProducto precio);
        Task ActualizarPrecioAsync(ActualizarPrecioRequest request);
        Task<decimal> GetPrecioVigenteAsync(int productoId, int monedaId);
        Task<decimal> GetPrecioFinalAsync(int productoId, int monedaId);
        Task<IEnumerable<PrecioProducto>> GetHistorialAsync(int productoId);
    }
}
