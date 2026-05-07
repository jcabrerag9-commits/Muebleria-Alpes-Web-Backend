namespace MuebleriaAlpesWebBackend.Domain.DTOs.Logistica
{
    public class CrearEnvioRequestDto
    {
        public int OrdenId { get; set; }
        public int DireccionId { get; set; }
        public string Transportista { get; set; } = string.Empty;
        public decimal CostoEnvio { get; set; }
    }
}
