using System;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    // CAMBIO ERP 2026-05-11: DTO para el Kardex Financiero SPA
    public class HistorialFinancieroDTO
    {
        public int MovimientoId { get; set; }
        public int OrdenId { get; set; }
        public int? PagoId { get; set; }
        public int FacturaId { get; set; }
        public string FacturaNumero { get; set; }
        public int ClienteId { get; set; }
        public int UsuarioId { get; set; }
        public string TipoMovimiento { get; set; } // PAGO, ANULACION_PAGO, etc.
        public string EstadoFactura { get; set; } // EMITIDA, PARCIAL, PAGADA, ANULADA
        public decimal Monto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
    }

    public class HistorialFiltroRequest
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? TipoMovimiento { get; set; }
        public int? FacturaId { get; set; }
        public int? ClienteId { get; set; }
        public int? UsuarioId { get; set; }
    }
}
