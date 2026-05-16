namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class FiltroCanalVentaResponse
    {
        public int CanalVentaId { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
    }
}
