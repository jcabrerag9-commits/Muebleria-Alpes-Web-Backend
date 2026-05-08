namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class FiltroOrdenDisponibleResponse
    {
        public int OrdenVentaId { get; set; }
        public string? NumeroOrden { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public int? ClienteDireccionId { get; set; }
        public int CanalVentaId { get; set; }
        public string? CanalVenta { get; set; }
        public decimal Total { get; set; }
        public string? EstadoOrden { get; set; }
        public DateTime? FechaOrden { get; set; }
    }
}
