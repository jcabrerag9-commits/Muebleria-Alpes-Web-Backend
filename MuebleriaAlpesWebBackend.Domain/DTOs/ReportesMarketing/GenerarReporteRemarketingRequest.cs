using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing
{
    public class GenerarReporteRemarketingRequest
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public int DiasInactividad { get; set; }

        public int? UsuarioId { get; set; }
    }
}