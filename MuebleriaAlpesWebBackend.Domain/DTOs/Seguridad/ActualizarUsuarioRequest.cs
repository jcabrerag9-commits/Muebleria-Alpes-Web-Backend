using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class ActualizarUsuarioRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = string.Empty;
    }
}