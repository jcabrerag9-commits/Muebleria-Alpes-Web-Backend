using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class CrearUsuarioRequest
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PasswordPlano { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ACTIVO";
    }
}