using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class BloquearUsuarioRequest
    {
        [Required]
        public int UsuarioId { get; set; }
    }
}