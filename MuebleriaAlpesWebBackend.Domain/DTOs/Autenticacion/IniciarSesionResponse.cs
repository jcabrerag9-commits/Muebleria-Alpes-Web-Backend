namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class IniciarSesionResponse
    {
        public int? UsuarioId { get; set; }

        public int? SesionId { get; set; }

        public string? TokenSesion { get; set; }

        public bool EsValido { get; set; }
    }
}