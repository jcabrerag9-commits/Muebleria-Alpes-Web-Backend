using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class AgregarDeduccionNominaDTO
    {
        [Required]
        public int TipoDeduccionId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
