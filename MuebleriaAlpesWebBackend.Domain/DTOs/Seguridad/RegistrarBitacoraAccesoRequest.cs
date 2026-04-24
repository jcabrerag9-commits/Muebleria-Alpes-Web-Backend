using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad
{
    public class RegistrarBitacoraAccesoRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ip { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [Required]
        [StringLength(30)]
        public string Resultado { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Detalle { get; set; }
    }
}