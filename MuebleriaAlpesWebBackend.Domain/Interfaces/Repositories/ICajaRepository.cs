using MuebleriaAlpesWebBackend.Domain.DTOs.Caja;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ICajaRepository
    {
        Task<BaseResponse<AbrirCajaDataDto>> AbrirCajaAsync(AbrirCajaRequestDto request);
        Task<BaseResponse> CerrarCajaAsync(CerrarCajaRequestDto request);
        Task<BaseResponse> ConciliarCajaAsync(ConciliarCajaRequestDto request);
    }
}
