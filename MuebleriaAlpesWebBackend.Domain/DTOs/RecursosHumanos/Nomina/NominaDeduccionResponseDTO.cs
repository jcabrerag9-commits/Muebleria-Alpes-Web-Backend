using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class NominaDeduccionResponseDTO
    {
        public int Id { get; set; }
        public int DetalleId { get; set; }
        public int TipoDeduccionId { get; set; }
        public string? TipoDeduccion { get; set; }
        public decimal Monto { get; set; }
    }
}
