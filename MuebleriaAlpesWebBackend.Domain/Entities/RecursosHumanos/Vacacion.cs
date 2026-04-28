using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Vacacion
    {
        public int VAC_VACACIONES { get; set; }
        public int EMP_EMPLEADO { get; set; }
        public DateTime VAC_FECHA_INICIO { get; set; }
        public DateTime VAC_FECHA_FIN { get; set; }
        public int VAC_DIAS_SOLICITADOS { get; set; }
        public string VAC_ESTADO { get; set; } = string.Empty;
        public DateTime VAC_FECHA_CREACION { get; set; }
    }
}
