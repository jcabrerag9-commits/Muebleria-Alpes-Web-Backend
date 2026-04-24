using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class CambiarEstadoEnvioRequest
    {
        [Required]
        public int EnvioId { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = string.Empty;
    }
}