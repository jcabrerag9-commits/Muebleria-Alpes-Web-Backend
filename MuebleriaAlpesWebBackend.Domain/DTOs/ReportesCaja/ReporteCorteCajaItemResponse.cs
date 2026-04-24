using System;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja
{
    public class ReporteCorteCajaItemResponse
    {
        public int CorteCajaId { get; set; }

        public DateTime? FechaCorte { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal MontoFinal { get; set; }

        public decimal TotalVentas { get; set; }

        public string? Observacion { get; set; }

        public string? Estado { get; set; }

        public decimal DiferenciaCaja { get; set; }
    }
}