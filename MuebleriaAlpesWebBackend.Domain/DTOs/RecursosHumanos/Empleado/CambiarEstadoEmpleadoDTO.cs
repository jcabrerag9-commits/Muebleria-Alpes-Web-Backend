using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado
{
    public class CambiarEstadoEmpleadoDTO
    {
        [Required]
        public string Estado { get; set; } = string.Empty;

        public string? Motivo { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
