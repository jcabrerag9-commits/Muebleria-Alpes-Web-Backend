using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Ventas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class VentasService : IVentasService
    {
        private readonly IVentasRepository _ventasRepository;

        public VentasService(IVentasRepository ventasRepository)
        {
            _ventasRepository = ventasRepository;
        }

        public async Task<BaseResponse<CrearOrdenDataDto>> CrearOrdenCompletaAsync(CrearOrdenRequestDto request)
        {
            if (request.ProductosIds == null || request.ProductosIds.Count == 0)
            {
                return new BaseResponse<CrearOrdenDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Debe incluir al menos un producto"
                };
            }

            if (request.ProductosIds.Count != request.Cantidades.Count)
            {
                return new BaseResponse<CrearOrdenDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "La cantidad de productos y cantidades no coinciden"
                };
            }

            if (request.Cantidades.Any(c => c <= 0))
            {
                return new BaseResponse<CrearOrdenDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Todas las cantidades deben ser mayores a cero"
                };
            }

            return await _ventasRepository.CrearOrdenCompletaAsync(request);
        }

        public async Task<BaseResponse> ActualizarEstadoOrdenAsync(ActualizarEstadoOrdenRequestDto request)
        {
            return await _ventasRepository.ActualizarEstadoOrdenAsync(request);
        }

        public async Task<BaseResponse> CancelarOrdenAsync(CancelarOrdenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Motivo))
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El motivo de cancelación es requerido"
                };
            }

            return await _ventasRepository.CancelarOrdenAsync(request);
        }

        public async Task<BaseResponse> AplicarPromocionAsync(AplicarPromocionRequestDto request)
        {
            return await _ventasRepository.AplicarPromocionAsync(request);
        }

        public async Task<BaseResponse<CalcularTotalesOrdenDataDto>> CalcularTotalesOrdenAsync(int ordenId)
        {
            return await _ventasRepository.CalcularTotalesOrdenAsync(ordenId);
        }
    }
}
