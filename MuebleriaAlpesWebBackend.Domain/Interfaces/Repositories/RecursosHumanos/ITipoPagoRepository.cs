using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface ITipoPagoRepository
    {
        Task<ResponseSpDTO> CrearAsync(CreateTipoPagoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTipoPagoDTO dto);
        Task<IEnumerable<ResponseTipoPagoDTO>> ListarAsync();
    }
}
