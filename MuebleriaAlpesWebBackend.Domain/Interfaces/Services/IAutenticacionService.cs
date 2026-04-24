using MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IAutenticacionService
    {
        Task<ValidarLoginResponse> ValidarLoginAsync(ValidarLoginRequest request);

        Task<IniciarSesionResponse> IniciarSesionAsync(IniciarSesionRequest request);

        Task<bool> CerrarSesionAsync(CerrarSesionRequest request);

        Task<GenerarTokenRecuperacionResponse> GenerarTokenRecuperacionAsync(GenerarTokenRecuperacionRequest request);

        Task<bool> MarcarTokenRecuperacionUsadoAsync(MarcarTokenRecuperacionUsadoRequest request);

        Task<SesionActivaResponse> SesionActivaAsync(int usuarioId);

        Task<TokenRecuperacionValidoResponse> TokenRecuperacionValidoAsync(string token);

        Task<ClientePorUsuarioResponse> ObtenerClientePorUsuarioAsync(int usuarioId);
    }
}