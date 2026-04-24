using System;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class ReporteIngresosCanalRequest
    {
        public int CanalVentaId { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
    }
}