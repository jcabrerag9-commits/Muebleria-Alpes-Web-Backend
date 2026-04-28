using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado
{
    public class AsignarTurnoDTO
    {
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int TurnoId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
