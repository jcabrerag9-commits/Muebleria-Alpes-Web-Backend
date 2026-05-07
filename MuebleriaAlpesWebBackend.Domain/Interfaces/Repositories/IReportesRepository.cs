using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Reportes;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IReportesRepository
    {
        Task<BaseResponse> RegistrarEjecucionReporteAsync(RegistrarEjecucionReporteRequestDto request);
    }
}
