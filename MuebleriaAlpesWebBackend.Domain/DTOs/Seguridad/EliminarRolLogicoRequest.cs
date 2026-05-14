using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class EliminarRolLogicoRequest
    {
        [Required]
        public int RolId { get; set; }
    }
}