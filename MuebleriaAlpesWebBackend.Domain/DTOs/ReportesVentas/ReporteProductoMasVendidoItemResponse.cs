namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas
{
    public class ReporteProductoMasVendidoItemResponse
    {
        public int? ProductoId { get; set; }

        public string? Sku { get; set; }

        public string? Producto { get; set; }

        public string? TipoMueble { get; set; }

        public decimal TotalUnidades { get; set; }

        public decimal TotalVendido { get; set; }
    }
}