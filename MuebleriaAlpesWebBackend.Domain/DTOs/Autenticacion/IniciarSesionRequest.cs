using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class IniciarSesionRequest
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PasswordPlano { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ip { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }
    }
}