using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class NominaDetalle
    {
        public int NOD_NOMINA_DETALLE { get; set; }
        public int RHN_NOMINA { get; set; }
        public int EMP_EMPLEADO { get; set; }
        public string? EMPLEADO { get; set; }
        public decimal NOD_SALARIO_BASE { get; set; }
        public decimal NOD_TOTAL_INGRESOS { get; set; }
        public decimal NOD_TOTAL_DEDUCCIONES { get; set; }
        public decimal NOD_SALARIO_NETO { get; set; }
        public string NOD_ESTADO { get; set; } = string.Empty;
    }
}
