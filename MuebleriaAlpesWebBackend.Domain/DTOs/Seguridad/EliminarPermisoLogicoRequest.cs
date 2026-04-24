using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class EliminarPermisoLogicoRequest
    {
        [Required]
        public int PermisoId { get; set; }
    }
}