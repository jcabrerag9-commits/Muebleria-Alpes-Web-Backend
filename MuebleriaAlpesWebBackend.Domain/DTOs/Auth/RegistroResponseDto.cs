namespace MuebleriaAlpesWebBackend.Domain.DTOs.Auth
{
    public class RegistroResponseDto
    {
        public int    Id        { get; set; }
        public string Username  { get; set; } = string.Empty;
        public string Rol       { get; set; } = "Cliente";
        /// <summary>CLI_CLIENTE de ALP_CLIENTE creado durante el registro. 0 si no se pudo crear.</summary>
        public int    ClienteId { get; set; }
    }
}
