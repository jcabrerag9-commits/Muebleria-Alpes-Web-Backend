using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado
{
    public class FinalizarAsignacionDTO
    {
        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
