using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado
{
    public class ResponseEmpleadoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string PrimerNombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
