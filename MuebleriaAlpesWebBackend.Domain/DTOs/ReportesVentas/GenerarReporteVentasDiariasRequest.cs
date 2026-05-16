using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class GenerarReporteVentasDiariasRequest
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public int? CiudadId { get; set; }

        public int? UsuarioId { get; set; }
    }
}