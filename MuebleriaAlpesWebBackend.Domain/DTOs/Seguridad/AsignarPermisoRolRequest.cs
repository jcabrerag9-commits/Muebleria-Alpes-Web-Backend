using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class AsignarPermisoRolRequest
    {
        [Required]
        public int RolId { get; set; }

        [Required]
        public int PermisoId { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ACTIVO";
    }
}