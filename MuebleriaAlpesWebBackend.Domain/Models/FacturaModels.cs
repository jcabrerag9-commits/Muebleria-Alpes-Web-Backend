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
        public DateTime FechaEmision { get; set; }
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

    public class FacturacionResponse<T>
    {
        public string Resultado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSuccess => Resultado == "EXITO";
    }
}
