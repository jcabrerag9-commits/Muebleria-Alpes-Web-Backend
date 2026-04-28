using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado
{
    public class CreateEmpleadoDTO
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public int TipoDocumentoId { get; set; }

        [Required]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        public string PrimerNombre { get; set; } = string.Empty;

        public string? SegundoNombre { get; set; }

        [Required]
        public string PrimerApellido { get; set; } = string.Empty;

        public string? SegundoApellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public DateTime FechaIngreso { get; set; }
    }
}
