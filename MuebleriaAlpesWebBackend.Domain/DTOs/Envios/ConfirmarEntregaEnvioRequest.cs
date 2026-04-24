using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class ConfirmarEntregaEnvioRequest
    {
        [Required]
        public int EnvioId { get; set; }

        public DateTime? FechaEntregaReal { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }
}