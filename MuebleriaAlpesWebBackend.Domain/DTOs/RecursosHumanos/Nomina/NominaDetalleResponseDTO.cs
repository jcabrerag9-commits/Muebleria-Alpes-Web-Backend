using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class NominaDetalleResponseDTO
    {
        public int Id { get; set; }
        public int NominaId { get; set; }
        public int EmpleadoId { get; set; }
        public string? Empleado { get; set; }
        public decimal SalarioBase { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal SalarioNeto { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
