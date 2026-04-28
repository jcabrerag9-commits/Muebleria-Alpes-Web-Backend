namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    /// <summary>
    /// Datos OUT adicionales de SP_CALCULAR_TOTALES_ORDEN.
    /// </summary>
    public class CalcularTotalesOrdenDataDto
    {
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Envio { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }
}
