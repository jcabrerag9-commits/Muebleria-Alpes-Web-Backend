namespace MuebleriaAlpesWebBackend.Domain.DTOs.Carrito
{
    /// <summary>
    /// Datos OUT adicionales de SP_CARRITO_CALCULAR_TOTAL.
    /// </summary>
    public class CalcularTotalCarritoDataDto
    {
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }
}
