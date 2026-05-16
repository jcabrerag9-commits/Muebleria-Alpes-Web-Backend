using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class AsignacionDepartamento
    {
        public int EDE_EMPLEADO_DEPARTAMENTO { get; set; }
        public string DEPARTAMENTO { get; set; } = string.Empty;
        public DateTime EDE_FECHA_INICIO { get; set; }
        public DateTime? EDE_FECHA_FIN { get; set; }
        public string EDE_ESTADO { get; set; } = string.Empty;
    }
}
