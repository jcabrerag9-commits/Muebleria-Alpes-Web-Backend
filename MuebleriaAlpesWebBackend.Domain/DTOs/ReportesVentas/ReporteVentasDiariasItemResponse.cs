using System;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class ReporteVentasDiariasItemResponse
    {
        public DateTime? FechaVenta { get; set; }

        public int TotalOrdenes { get; set; }

        public decimal TotalUnidades { get; set; }

        public decimal TotalVendido { get; set; }
    }
}