using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoDeduccion
{
    public class UpdateTipoDeduccionDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
    }
}
