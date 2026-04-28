using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones
{
    public class CambiarEstadoVacacionesDTO
    {
        public int UsuarioId { get; set; }
        public string? Motivo { get; set; }
    }
}
