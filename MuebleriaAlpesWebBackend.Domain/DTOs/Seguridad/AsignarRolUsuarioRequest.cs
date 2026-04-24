using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class AsignarRolUsuarioRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int RolId { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ACTIVO";
    }
}