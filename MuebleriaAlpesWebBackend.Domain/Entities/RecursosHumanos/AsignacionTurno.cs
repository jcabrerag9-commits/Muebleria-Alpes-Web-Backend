using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class AsignacionTurno
    {
        public int ETU_EMPLEADO_TURNO { get; set; }
        public string TURNO { get; set; } = string.Empty;
        public DateTime RHT_HORA_INICIO { get; set; }
        public DateTime RHT_HORA_FIN { get; set; }
        public DateTime ETU_FECHA_INICIO { get; set; }
        public DateTime? ETU_FECHA_FIN { get; set; }
        public string ETU_ESTADO { get; set; } = string.Empty;
    }
}
