using System;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class FacturaDTO
    {
        public int FacturaId { get; set; }
        public int OrdenId { get; set; }
        public int ClienteId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Anulable { get; set; }
        public DateTime FechaEmision { get; set; }
        public string? NumeroOrden { get; set; }
        public int? FormaPagoId { get; set; }
        public string? FormaPagoNombre { get; set; }
        public string? Nit { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteNit { get; set; }
        public string? ClienteCorreo { get; set; }
        public string? ClienteTelefono { get; set; }
        public string? ClienteDireccion { get; set; }
        public System.Collections.Generic.List<FacturaDetalleDTO> Detalles { get; set; } = new();
    }

    public class FacturaDetalleDTO
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class GenerarFacturaRequest
    {
        public int OrdenId { get; set; }
        public int PagoId { get; set; }
        public int UsuarioId { get; set; }
    }

    public class AnularFacturaRequest
    {
        public int FacturaId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }


    public class OrdenPendienteDTO
    {
        public int OrdenId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
