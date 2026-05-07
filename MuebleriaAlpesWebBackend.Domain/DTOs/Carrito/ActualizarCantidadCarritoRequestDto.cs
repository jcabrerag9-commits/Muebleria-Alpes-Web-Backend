namespace MuebleriaAlpesWebBackend.Domain.DTOs.Carrito
{
    public class ActualizarCantidadCarritoRequestDto
    {
        public int DetalleId { get; set; }
        public int NuevaCantidad { get; set; }
    }
}
