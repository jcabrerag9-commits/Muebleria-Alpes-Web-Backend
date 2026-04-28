using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Asistencia
    {
        public int RHA_ASISTENCIA { get; set; }
        public int EMP_EMPLEADO { get; set; }
        public DateTime RHA_FECHA { get; set; }
        public DateTime? RHA_HORA_ENTRADA { get; set; }
        public DateTime? RHA_HORA_SALIDA { get; set; }
        public string RHA_ESTADO { get; set; } = string.Empty;
    }
}
