using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class AgregarEmpleadoNominaDTO
    {
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public decimal SalarioBase { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
