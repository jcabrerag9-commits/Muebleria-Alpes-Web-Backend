using MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IAutenticacionRepository _autenticacionRepository;

        public AutenticacionService(IAutenticacionRepository autenticacionRepository)
        {
            _autenticacionRepository = autenticacionRepository;
        }

        public async Task<ValidarLoginResponse> ValidarLoginAsync(ValidarLoginRequest request)
        {
            return await _autenticacionRepository.ValidarLoginAsync(request);
        }

        public async Task<IniciarSesionResponse> IniciarSesionAsync(IniciarSesionRequest request)
        {
            return await _autenticacionRepository.IniciarSesionAsync(request);
        }

        public async Task<bool> CerrarSesionAsync(CerrarSesionRequest request)
        {
            return await _autenticacionRepository.CerrarSesionAsync(request);
        }

        public async Task<GenerarTokenRecuperacionResponse> GenerarTokenRecuperacionAsync(GenerarTokenRecuperacionRequest request)
        {
            return await _autenticacionRepository.GenerarTokenRecuperacionAsync(request);
        }

        public async Task<bool> MarcarTokenRecuperacionUsadoAsync(MarcarTokenRecuperacionUsadoRequest request)
        {
            return await _autenticacionRepository.MarcarTokenRecuperacionUsadoAsync(request);
        }

        public async Task<SesionActivaResponse> SesionActivaAsync(int usuarioId)
        {
            return await _autenticacionRepository.SesionActivaAsync(usuarioId);
        }

        public async Task<TokenRecuperacionValidoResponse> TokenRecuperacionValidoAsync(string token)
        {
            return await _autenticacionRepository.TokenRecuperacionValidoAsync(token);
        }

        public async Task<ClientePorUsuarioResponse> ObtenerClientePorUsuarioAsync(int usuarioId)
        {
            return await _autenticacionRepository.ObtenerClientePorUsuarioAsync(usuarioId);
        }
    }
}