namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    public class CancelarOrdenRequestDto
    {
        public int OrdenId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}
