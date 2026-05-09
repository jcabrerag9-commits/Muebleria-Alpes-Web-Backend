using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface IVacacionesService
    {
        Task<ResponseSpDTO> SolicitarAsync(SolicitarVacacionesDTO dto);
        Task<ResponseSpDTO> AprobarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<ResponseSpDTO> RechazarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<ResponseSpDTO> CancelarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<IEnumerable<VacacionResponseDTO>> ListarAsync(int empleadoId, string? estado);
    }
}
