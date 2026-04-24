namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing
{
    public class ReporteRetencionItemResponse
    {
        public string? Periodo { get; set; }

        public int ClientesBase { get; set; }

        public int ClientesRetenidos { get; set; }

        public decimal TasaRetencion { get; set; }
    }
}