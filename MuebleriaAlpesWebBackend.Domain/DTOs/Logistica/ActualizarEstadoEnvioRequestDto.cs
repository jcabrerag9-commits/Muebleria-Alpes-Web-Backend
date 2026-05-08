namespace MuebleriaAlpesWebBackend.Domain.DTOs.Logistica
{
    public class ActualizarEstadoEnvioRequestDto
    {
        public int EnvioId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
        public string? Guia { get; set; }
    }
}
