using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion
{
    public class GenerarTokenRecuperacionRequest
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }
    }
}