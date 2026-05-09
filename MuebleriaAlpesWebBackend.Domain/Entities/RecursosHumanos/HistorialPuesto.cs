using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class HistorialPuesto
    {
        public int EPU_EMPLEADO_PUESTO { get; set; }
        public string PUESTO { get; set; } = string.Empty;
        public decimal EPU_SALARIO { get; set; }
        public DateTime EPU_FECHA_INICIO { get; set; }
        public DateTime? EPU_FECHA_FIN { get; set; }
        public string EPU_ESTADO { get; set; } = string.Empty;
    }
}
