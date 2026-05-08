using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class DesbloquearUsuarioRequest
    {
        [Required]
        public int UsuarioId { get; set; }
    }
}