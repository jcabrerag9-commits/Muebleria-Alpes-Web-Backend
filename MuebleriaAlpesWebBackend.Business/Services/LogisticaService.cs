using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Logistica;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class LogisticaService : ILogisticaService
    {
        private readonly ILogisticaRepository _logisticaRepository;

        public LogisticaService(ILogisticaRepository logisticaRepository)
        {
            _logisticaRepository = logisticaRepository;
        }

        public async Task<BaseResponse<CrearEnvioDataDto>> CrearEnvioAsync(CrearEnvioRequestDto request)
        {
            if (request.OrdenId <= 0)
            {
                return new BaseResponse<CrearEnvioDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID de la orden debe ser mayor a cero."
                };
            }

            if (request.DireccionId <= 0)
            {
                return new BaseResponse<CrearEnvioDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID de la dirección debe ser mayor a cero."
                };
            }

            return await _logisticaRepository.CrearEnvioAsync(request);
        }

        public async Task<BaseResponse> ActualizarEstadoEnvioAsync(ActualizarEstadoEnvioRequestDto request)
        {
            if (request.EnvioId <= 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID del envío debe ser mayor a cero."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NuevoEstado))
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El nuevo estado es requerido."
                };
            }

            return await _logisticaRepository.ActualizarEstadoEnvioAsync(request);
        }
    }
}
