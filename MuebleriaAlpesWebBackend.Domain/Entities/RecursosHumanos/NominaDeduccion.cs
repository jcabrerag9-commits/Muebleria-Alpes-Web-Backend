using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class NominaDeduccion
    {
        public int NDE_NOMINA_DEDUCCION { get; set; }
        public int NOD_NOMINA_DETALLE { get; set; }
        public int TDE_TIPO_DEDUCCION { get; set; }
        public string? TIPO_DEDUCCION { get; set; }
        public decimal NDE_MONTO { get; set; }
    }
}
