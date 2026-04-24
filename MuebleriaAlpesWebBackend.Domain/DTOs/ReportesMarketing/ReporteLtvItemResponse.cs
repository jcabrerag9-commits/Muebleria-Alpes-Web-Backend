namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing
{
    public class ReporteLtvItemResponse
    {
        public int ClienteId { get; set; }

        public string? ClienteCodigo { get; set; }

        public string? ClienteNombre { get; set; }

        public decimal Ltv { get; set; }
    }
}