namespace MuebleriaAlpesWebBackend.Domain.DTOs.Admin
{
    public class ListarOrdenesFiltroDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? EstadoId { get; set; }
    }

    public class AdminOrdenDto
    {
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public DateTime FechaOrden { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class AdminListarOrdenesDataDto
    {
        public List<AdminOrdenDto> Ordenes { get; set; } = new();
    }

    public class AdminPagoDto
    {
        public int PagoId { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
    }

    public class AdminListarPagosDataDto
    {
        public List<AdminPagoDto> Pagos { get; set; } = new();
    }

    public class AdminFacturaDto
    {
        public int FacturaId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string NumeroOrden { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
    }

    public class AdminListarFacturasDataDto
    {
        public List<AdminFacturaDto> Facturas { get; set; } = new();
    }
}
