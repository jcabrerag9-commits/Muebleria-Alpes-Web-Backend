using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina
{
    public class NominaIngresoResponseDTO
    {
        public int Id { get; set; }
        public int DetalleId { get; set; }
        public int TipoPagoId { get; set; }
        public string? TipoPago { get; set; }
        public decimal Monto { get; set; }
    }
}
