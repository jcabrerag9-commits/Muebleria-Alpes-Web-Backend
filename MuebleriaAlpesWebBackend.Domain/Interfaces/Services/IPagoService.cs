using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IPagoService
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request);
        Task<object?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<object>> ObtenerTodosAsync();
        Task<IEnumerable<object>> ObtenerPorFacturaIdAsync(int facturaId);
        Task<bool> AnularPagoAsync(int id);
    }
}
