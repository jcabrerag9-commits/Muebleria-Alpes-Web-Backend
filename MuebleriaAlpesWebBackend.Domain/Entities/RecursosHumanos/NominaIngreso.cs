using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class NominaIngreso
    {
        public int NNI_NOMINA_INGRESO { get; set; }
        public int NOD_NOMINA_DETALLE { get; set; }
        public int TIP_TIPO_PAGO { get; set; }
        public string? TIPO_PAGO { get; set; }
        public decimal NNI_MONTO { get; set; }
    }
}
