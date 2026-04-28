using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion
{
    public class EvaluacionResponseDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string? CodigoEmpleado { get; set; }
        public string? NombreEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Calificacion { get; set; }
        public string? Comentarios { get; set; }
    }
}
