using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Evaluacion
    {
        public int EVL_EVALUACION { get; set; }
        public int EMP_EMPLEADO { get; set; }
        public string? EMP_CODIGO { get; set; }
        public string? NOMBRE_EMPLEADO { get; set; }
        public DateTime EVL_FECHA_EVALUACION { get; set; }
        public decimal? EVL_CALIFICACION { get; set; }
        public string? EVL_COMENTARIOS { get; set; }
    }
}
