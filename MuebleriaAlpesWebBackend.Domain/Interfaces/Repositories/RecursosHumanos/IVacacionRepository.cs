using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface IVacacionRepository
    {
        Task<ResponseSpDTO> SolicitarAsync(SolicitarVacacionesDTO dto);
        Task<ResponseSpDTO> AprobarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<ResponseSpDTO> RechazarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<ResponseSpDTO> CancelarAsync(int id, CambiarEstadoVacacionesDTO dto);
        Task<IEnumerable<VacacionResponseDTO>> ListarAsync(int empleadoId, string? estado);
    }
}
