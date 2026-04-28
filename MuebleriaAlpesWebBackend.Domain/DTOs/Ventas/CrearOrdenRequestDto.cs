namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    public class CrearOrdenRequestDto
    {
        public int ClienteId { get; set; }
        public int CanalVenta { get; set; }
        public List<int> ProductosIds { get; set; } = new();
        public List<int> Cantidades { get; set; } = new();
    }
}
