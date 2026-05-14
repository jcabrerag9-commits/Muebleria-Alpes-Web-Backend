namespace MuebleriaAlpesWebBackend.Domain.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        /// <summary>CLI_CLIENTE de ALP_CLIENTE — 0 si el usuario no tiene cliente asociado.</summary>
        public int ClienteId { get; set; }
    }
}
