using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Reportes;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IReportesService
    {
        Task<BaseResponse> RegistrarEjecucionReporteAsync(RegistrarEjecucionReporteRequestDto request);
    }
}
