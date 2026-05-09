using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Logistica;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface ILogisticaService
    {
        Task<BaseResponse<CrearEnvioDataDto>> CrearEnvioAsync(CrearEnvioRequestDto request);
        Task<BaseResponse> ActualizarEstadoEnvioAsync(ActualizarEstadoEnvioRequestDto request);
    }
}
