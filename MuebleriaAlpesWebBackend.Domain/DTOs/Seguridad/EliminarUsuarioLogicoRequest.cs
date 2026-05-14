using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class EliminarUsuarioLogicoRequest
    {
        [Required]
        public int UsuarioId { get; set; }
    }
}