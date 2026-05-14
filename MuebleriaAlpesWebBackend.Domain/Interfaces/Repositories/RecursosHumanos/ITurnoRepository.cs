using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface ITurnoRepository
    {
        Task<ResponseSpDTO> CrearAsync(CreateTurnoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTurnoDTO dto);
        Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoTurnoDTO dto);
        Task<ResponseTurnoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ResponseTurnoDTO>> ListarAsync(bool soloActivos);
    }
}
