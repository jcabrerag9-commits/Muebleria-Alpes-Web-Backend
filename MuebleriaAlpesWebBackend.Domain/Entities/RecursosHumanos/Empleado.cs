using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos
{
    public class Empleado
    {
        public int EMP_EMPLEADO { get; set; }
        public string EMP_CODIGO { get; set; } = string.Empty;
        public int TDO_TIPO_DOCUMENTO { get; set; }
        public string? TIPO_DOCUMENTO { get; set; }
        public string EMP_NUMERO_DOCUMENTO { get; set; } = string.Empty;
        public string EMP_PRIMER_NOMBRE { get; set; } = string.Empty;
        public string? EMP_SEGUNDO_NOMBRE { get; set; }
        public string EMP_PRIMER_APELLIDO { get; set; } = string.Empty;
        public string? EMP_SEGUNDO_APELLIDO { get; set; }
        public string EMP_EMAIL { get; set; } = string.Empty;
        public string? EMP_TELEFONO { get; set; }
        public DateTime EMP_FECHA_NACIMIENTO { get; set; }
        public DateTime EMP_FECHA_INGRESO { get; set; }
        public string EMP_ESTADO { get; set; } = string.Empty;

        // Para SP_LISTAR_EMPLEADOS
        public string? NOMBRE_COMPLETO { get; set; }
    }
}
