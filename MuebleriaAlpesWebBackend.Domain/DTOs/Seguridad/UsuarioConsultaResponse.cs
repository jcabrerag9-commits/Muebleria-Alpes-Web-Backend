namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class UsuarioConsultaResponse
    {
        public int UsuarioId { get; set; }
        public string? Codigo { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
