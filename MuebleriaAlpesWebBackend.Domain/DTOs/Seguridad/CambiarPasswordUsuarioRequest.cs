using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class CambiarPasswordUsuarioRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(200)]
        public string PasswordPlano { get; set; } = string.Empty;
    }
}