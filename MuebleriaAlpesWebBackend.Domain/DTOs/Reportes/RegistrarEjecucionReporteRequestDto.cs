namespace MuebleriaAlpesWebBackend.Domain.DTOs.Reportes
{
    public class RegistrarEjecucionReporteRequestDto
    {
        public int UsuarioId { get; set; }
        public string NombreReporte { get; set; } = string.Empty;
        public string Parametros { get; set; } = string.Empty; // Mapped to CLOB
        public int TiempoMs { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
