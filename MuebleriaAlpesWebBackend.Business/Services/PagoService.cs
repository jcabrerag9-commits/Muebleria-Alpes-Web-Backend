using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;

        public PagoService(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request)
        {
            // El servicio solo orquesta la llamada al repositorio
            // La lógica de negocio reside en el SP
            return await _pagoRepository.ProcesarPagoAsync(request);
        }
    }
}
