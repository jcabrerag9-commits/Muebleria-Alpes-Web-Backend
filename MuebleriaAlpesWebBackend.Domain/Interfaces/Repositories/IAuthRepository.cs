using MuebleriaAlpesWebBackend.Domain.DTOs.Auth;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto?> LoginAsync(string username, string password);
        Task<AuthDiagnosticoDto> DiagnosticoAsync(string username, string passwordPrueba);
        Task<(bool Ok, string Mensaje, RegistroResponseDto? Data)> RegistrarAsync(RegistroRequestDto request);
        /// <summary>
        /// Busca o crea un CLI_CLIENTE en ALP_CLIENTE para el usuario con el username dado.
        /// Usa CLI_NUMERO_DOCUMENTO = username como clave de búsqueda.
        /// </summary>
        Task<int> EnsureClienteAsync(string username);
    }
}
