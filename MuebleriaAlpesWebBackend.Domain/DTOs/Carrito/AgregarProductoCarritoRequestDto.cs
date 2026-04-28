namespace MuebleriaAlpesWebBackend.Domain.DTOs.Carrito
{
    public class AgregarProductoCarritoRequestDto
    {
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
