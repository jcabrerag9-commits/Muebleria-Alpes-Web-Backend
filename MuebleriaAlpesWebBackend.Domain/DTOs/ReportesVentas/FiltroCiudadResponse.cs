namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class FiltroCiudadResponse
    {
        public int CiudadId { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public int DepartamentoId { get; set; }
        public string? Estado { get; set; }
    }
}
