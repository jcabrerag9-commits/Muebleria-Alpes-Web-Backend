using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Asistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface IAsistenciaRepository
    {
        Task<ResponseSpDTO> RegistrarAsync(RegistrarAsistenciaDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarAsistenciaDTO dto);
        Task<IEnumerable<AsistenciaResponseDTO>> ListarAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin);
    }
}
