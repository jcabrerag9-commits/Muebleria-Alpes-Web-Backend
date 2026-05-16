using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Puesto
    {
        public int RHP_PUESTO { get; set; }
        public string RHP_CODIGO { get; set; } = string.Empty;
        public string RHP_NOMBRE { get; set; } = string.Empty;
        public string? RHP_DESCRIPCION { get; set; }
        public string RHP_ESTADO { get; set; } = string.Empty;
    }
}
