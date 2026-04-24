using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IPagoRepository
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request);
    }
}
