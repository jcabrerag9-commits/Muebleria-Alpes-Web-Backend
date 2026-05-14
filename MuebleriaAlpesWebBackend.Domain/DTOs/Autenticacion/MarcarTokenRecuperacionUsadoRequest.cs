using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class MarcarTokenRecuperacionUsadoRequest
    {
        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;
    }
}