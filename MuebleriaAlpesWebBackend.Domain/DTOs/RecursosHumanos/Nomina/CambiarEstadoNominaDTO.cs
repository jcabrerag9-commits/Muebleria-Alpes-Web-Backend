using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class CambiarEstadoNominaDTO
    {
        [Required]
        public string Estado { get; set; } = string.Empty;

        [Required]
        public int UsuarioId { get; set; }
    }
}
