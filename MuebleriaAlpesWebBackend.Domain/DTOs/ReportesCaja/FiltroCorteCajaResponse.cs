namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja
{
    public class FiltroCorteCajaResponse
    {
        public int CorteCajaId { get; set; }
        public DateTime? FechaCorte { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal MontoFinal { get; set; }
        public decimal TotalVentas { get; set; }
        public string? Estado { get; set; }
    }
}
