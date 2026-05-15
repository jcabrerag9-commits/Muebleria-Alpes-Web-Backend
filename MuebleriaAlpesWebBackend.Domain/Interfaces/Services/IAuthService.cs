using MuebleriaAlpesWebBackend.Domain.DTOs.Auth;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<BaseResponse<RegistroResponseDto>> RegistrarAsync(RegistroRequestDto request);
        Task<int> EnsureClienteAsync(string username);
    }
}
