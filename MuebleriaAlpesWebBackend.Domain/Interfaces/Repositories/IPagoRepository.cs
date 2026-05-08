using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IPagoRepository
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, System.Data.IDbTransaction transaction = null);
        Task<decimal> ObtenerSaldoPendienteAsync(int facturaId, System.Data.IDbTransaction transaction = null);
        Task<object?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<object>> ObtenerTodosAsync();
        Task<IEnumerable<object>> ObtenerPorFacturaIdAsync(int facturaId);
        Task<bool> AnularPagoAsync(int id);
    }
}
