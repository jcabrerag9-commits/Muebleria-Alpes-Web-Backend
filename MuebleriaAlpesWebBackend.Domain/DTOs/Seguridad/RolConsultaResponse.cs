namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class RolConsultaResponse
    {
        public int RolId { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
