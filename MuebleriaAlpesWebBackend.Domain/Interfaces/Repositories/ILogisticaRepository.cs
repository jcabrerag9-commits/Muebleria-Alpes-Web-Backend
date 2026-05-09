using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Logistica;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ILogisticaRepository
    {
        Task<BaseResponse<CrearEnvioDataDto>> CrearEnvioAsync(CrearEnvioRequestDto request);
        Task<BaseResponse> ActualizarEstadoEnvioAsync(ActualizarEstadoEnvioRequestDto request);
    }
}
