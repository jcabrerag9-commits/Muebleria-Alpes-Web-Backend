using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Asistencia
{
    public class RegistrarAsistenciaDTO
    {
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public DateTime? HoraEntrada { get; set; }

        public DateTime? HoraSalida { get; set; }

        [Required]
        public string Estado { get; set; } = string.Empty;

        [Required]
        public int UsuarioId { get; set; }
    }
}
