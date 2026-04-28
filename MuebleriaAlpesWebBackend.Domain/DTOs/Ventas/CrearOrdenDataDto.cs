namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    /// <summary>
    /// Datos OUT adicionales de SP_CREAR_ORDEN_COMPLETA.
    /// </summary>
    public class CrearOrdenDataDto
    {
        public int? OrdenId { get; set; }
        public decimal Total { get; set; }
    }
}
