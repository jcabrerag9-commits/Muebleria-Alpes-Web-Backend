using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Turno
    {
        public int RHT_TURNO { get; set; }
        public string RHT_NOMBRE { get; set; } = string.Empty;
        public DateTime RHT_HORA_INICIO { get; set; }
        public DateTime RHT_HORA_FIN { get; set; }
        public string RHT_ESTADO { get; set; } = string.Empty;
    }
}
