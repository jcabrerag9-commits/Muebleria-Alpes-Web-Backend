using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class QuitarPermisoRolRequest
    {
        [Required]
        public int RolPermisoId { get; set; }
    }
}