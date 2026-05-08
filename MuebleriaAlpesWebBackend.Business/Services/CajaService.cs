using MuebleriaAlpesWebBackend.Domain.DTOs.Caja;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _cajaRepository;

        public CajaService(ICajaRepository cajaRepository)
        {
            _cajaRepository = cajaRepository;
        }

        public async Task<BaseResponse<AbrirCajaDataDto>> AbrirCajaAsync(AbrirCajaRequestDto request)
        {
            if (request.MontoInicial < 0)
            {
                return new BaseResponse<AbrirCajaDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "El monto inicial no puede ser negativo."
                };
            }

            return await _cajaRepository.AbrirCajaAsync(request);
        }

        public async Task<BaseResponse> CerrarCajaAsync(CerrarCajaRequestDto request)
        {
            if (request.CorteId <= 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID del corte debe ser mayor a cero."
                };
            }

            if (request.MontoFinal < 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El monto final no puede ser negativo."
                };
            }

            return await _cajaRepository.CerrarCajaAsync(request);
        }

        public async Task<BaseResponse> ConciliarCajaAsync(ConciliarCajaRequestDto request)
        {
            if (request.CorteId <= 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID del corte debe ser mayor a cero."
                };
            }

            return await _cajaRepository.ConciliarCajaAsync(request);
        }
    }
}
