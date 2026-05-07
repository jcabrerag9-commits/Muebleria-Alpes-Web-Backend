using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Departamento
{
    public class CreateDepartamentoDTO
    {
        [Required(ErrorMessage = "El código es requerido.")]
        [MinLength(2, ErrorMessage = "El código debe tener al menos 2 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }
    }
}
