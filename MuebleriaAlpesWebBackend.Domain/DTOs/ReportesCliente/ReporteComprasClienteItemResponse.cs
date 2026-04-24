using System;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente
{
    public class ReporteComprasClienteItemResponse
    {
        public int OrdenVentaId { get; set; }

        public string? NumeroOrden { get; set; }

        public DateTime? FechaOrden { get; set; }

        public decimal ValorCompra { get; set; }

        public string? FormaPago { get; set; }

        public int? ProductoId { get; set; }

        public string? Sku { get; set; }

        public string? Mueble { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal TotalLinea { get; set; }
    }
}