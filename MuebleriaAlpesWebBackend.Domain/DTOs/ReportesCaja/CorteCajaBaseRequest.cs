using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja
{
    public class CorteCajaBaseRequest
    {
        [Required]
        public int CorteCajaId { get; set; }
    }
}