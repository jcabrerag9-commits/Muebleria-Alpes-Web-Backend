using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Ventas;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IVentasService
    {
        Task<BaseResponse<CrearOrdenDataDto>> CrearOrdenCompletaAsync(CrearOrdenRequestDto request);
        Task<BaseResponse> ActualizarEstadoOrdenAsync(ActualizarEstadoOrdenRequestDto request);
        Task<BaseResponse> CancelarOrdenAsync(CancelarOrdenRequestDto request);
        Task<BaseResponse> AplicarPromocionAsync(AplicarPromocionRequestDto request);
        Task<BaseResponse<CalcularTotalesOrdenDataDto>> CalcularTotalesOrdenAsync(int ordenId);
    }
}
