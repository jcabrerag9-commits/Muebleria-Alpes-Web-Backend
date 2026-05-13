using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class CerrarSesionRequest
    {
        [Required]
        [StringLength(500)]
        public string TokenSesion { get; set; } = string.Empty;
    }
}