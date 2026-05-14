using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class TipoDeduccion
    {
        public int TDE_TIPO_DEDUCCION { get; set; }
        public string TDE_CODIGO { get; set; } = string.Empty;
        public string TDE_NOMBRE { get; set; } = string.Empty;
    }
}
