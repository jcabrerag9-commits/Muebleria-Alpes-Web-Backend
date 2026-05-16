using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno
{
    public class CambiarEstadoTurnoDTO
    {
        [Required]
        public string Estado { get; set; } = string.Empty;
    }
}
