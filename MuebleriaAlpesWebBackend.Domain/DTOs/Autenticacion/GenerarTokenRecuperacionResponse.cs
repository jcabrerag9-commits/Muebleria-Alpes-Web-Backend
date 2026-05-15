namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class GenerarTokenRecuperacionResponse
    {
        public int RecuperacionClaveId { get; set; }

        public string? Token { get; set; }
    }
}