using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion
{
    public class ActualizarEvaluacionDTO
    {
        [Range(0, 100)]
        public decimal? Calificacion { get; set; }

        public string? Comentarios { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
