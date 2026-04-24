using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class ActualizarEnvioRequest
    {
        [Required]
        public int EnvioId { get; set; }

        [Required]
        public int ClienteDireccionId { get; set; }

        [StringLength(100)]
        public string? NumeroGuia { get; set; }

        [StringLength(150)]
        public string? Transportista { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El costo de envío no puede ser negativo.")]
        public decimal? CostoEnvio { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaEntregaEstimada { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }
}