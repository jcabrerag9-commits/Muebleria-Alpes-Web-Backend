using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class TipoPago
    {
        public int TIP_TIPO_PAGO { get; set; }
        public string TIP_CODIGO { get; set; } = string.Empty;
        public string TIP_NOMBRE { get; set; } = string.Empty;
    }
}
