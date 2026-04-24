using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IPagoService
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request);
    }
}
