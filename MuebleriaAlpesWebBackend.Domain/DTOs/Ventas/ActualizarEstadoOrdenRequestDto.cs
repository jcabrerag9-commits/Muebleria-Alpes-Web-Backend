namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    public class ActualizarEstadoOrdenRequestDto
    {
        public int OrdenId { get; set; }
        public int NuevoEstado { get; set; }
        public int UsuarioId { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
