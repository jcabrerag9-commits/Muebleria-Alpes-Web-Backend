using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Nomina
    {
        public int RHN_NOMINA { get; set; }
        public string RHN_PERIODO { get; set; } = string.Empty;
        public DateTime RHN_FECHA_INICIO { get; set; }
        public DateTime RHN_FECHA_FIN { get; set; }
        public string RHN_ESTADO { get; set; } = string.Empty;
    }
}
