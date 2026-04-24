using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class QuitarRolUsuarioRequest
    {
        [Required]
        public int UsuarioRolId { get; set; }
    }
}