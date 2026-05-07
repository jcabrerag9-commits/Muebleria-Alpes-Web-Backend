using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado
{
    public class UpdateEmpleadoDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telefono { get; set; }
    }
}
