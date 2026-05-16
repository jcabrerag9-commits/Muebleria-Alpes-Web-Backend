using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoDeduccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface ITipoDeduccionService
    {
        Task<ResponseSpDTO> CrearAsync(CreateTipoDeduccionDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTipoDeduccionDTO dto);
        Task<IEnumerable<ResponseTipoDeduccionDTO>> ListarAsync();
    }
}
