using System;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class PagoDTO
    {
        public int PagoId { get; set; }
        public string FacturaNumero { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class ProcesarPagoRequest
    {
        public int? FacturaId { get; set; }
        public int? OrdenId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal Monto { get; set; }
        public int MonedaId { get; set; } = 1;
        public string Referencia { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
    }

    public class ProcesarPagoData
    {
        public int? PagoId { get; set; }
        public int? FacturaId { get; set; }
    }

    public class ProcesarPagoResponse : ApiResponse<ProcesarPagoData>
    {
        // Propiedades de conveniencia para mantener compatibilidad con lógica de servicios
        public int? PagoId => Data?.PagoId;
        public int? FacturaId => Data?.FacturaId;
    }

    public class OrdenPendientePagoDTO
    {
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string FacturaNumero { get; set; } = string.Empty;
        public decimal TotalFactura { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
    }

    public class AnularPagoRequest
    {
        public int PagoId { get; set; }
        public string Motivo { get; set; }
        public int? UsuarioId { get; set; }
    }
}
