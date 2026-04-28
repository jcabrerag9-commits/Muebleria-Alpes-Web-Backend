using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Departamento
    {
        public int RHD_DEPARTAMENTO { get; set; }
        public string RHD_CODIGO { get; set; } = string.Empty;
        public string RHD_NOMBRE { get; set; } = string.Empty;
        public string? RHD_DESCRIPCION { get; set; }
    }
}
