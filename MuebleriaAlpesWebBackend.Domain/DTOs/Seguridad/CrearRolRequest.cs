using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class CrearRolRequest
    {
        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ACTIVO";
    }
}
