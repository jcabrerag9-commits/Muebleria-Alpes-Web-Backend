namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing
{
    public class ReporteCohorteItemResponse
    {
        public string? Cohorte { get; set; }

        public int TotalClientes { get; set; }

        public int TotalCompradores { get; set; }

        public decimal TasaConversion { get; set; }
    }
}