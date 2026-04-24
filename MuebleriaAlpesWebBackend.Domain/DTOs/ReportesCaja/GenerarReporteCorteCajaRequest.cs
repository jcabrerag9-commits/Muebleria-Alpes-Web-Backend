using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja
{
    public class GenerarReporteCorteCajaRequest
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public string? Estado { get; set; }

        public int? UsuarioId { get; set; }
    }
}