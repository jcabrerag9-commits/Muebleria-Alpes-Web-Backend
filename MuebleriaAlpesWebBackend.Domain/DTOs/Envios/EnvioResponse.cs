using System;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class EnvioResponse
    {
        public int EnvioId { get; set; }

        public int OrdenVentaId { get; set; }

        public int ClienteDireccionId { get; set; }

        public string? NumeroGuia { get; set; }

        public string? Transportista { get; set; }

        public decimal? CostoEnvio { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaEntregaEstimada { get; set; }

        public DateTime? FechaEntregaReal { get; set; }

        public string? Estado { get; set; }

        public string? Observaciones { get; set; }
    }
}