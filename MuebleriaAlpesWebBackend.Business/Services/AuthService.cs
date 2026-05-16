using MuebleriaAlpesWebBackend.Domain.DTOs.Auth;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<BaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _authRepository.LoginAsync(request.Username, request.Password);

            if (usuario == null)
            {
                return new BaseResponse<LoginResponseDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Credenciales inválidas"
                };
            }

            return new BaseResponse<LoginResponseDto>
            {
                Resultado = "EXITO",
                Mensaje = "Login exitoso",
                Data = usuario
            };
        }

        public async Task<BaseResponse<RegistroResponseDto>> RegistrarAsync(RegistroRequestDto request)
        {
            var (ok, mensaje, data) = await _authRepository.RegistrarAsync(request);

            if (ok)
                return new BaseResponse<RegistroResponseDto> { Resultado = "EXITO", Mensaje = mensaje, Data = data };

            return new BaseResponse<RegistroResponseDto> { Resultado = "ERROR", Mensaje = mensaje };
        }

        public async Task<int> EnsureClienteAsync(string username)
            => await _authRepository.EnsureClienteAsync(username);
    }
}
