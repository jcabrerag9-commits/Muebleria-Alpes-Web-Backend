using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno
{
    public class CreateTurnoDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        public DateTime HoraFin { get; set; }
    }
}
