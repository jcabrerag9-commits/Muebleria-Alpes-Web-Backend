using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface IAsignacionEmpleadoRepository
    {
        Task<ResponseSpDTO> AsignarDepartamentoAsync(AsignarDepartamentoDTO dto);
        Task<ResponseSpDTO> FinalizarDepartamentoAsync(int asignacionId, FinalizarAsignacionDTO dto);
        Task<IEnumerable<AsignarDepartamentoResponseDTO>> ListarDepartamentosAsync(int empleadoId);

        Task<ResponseSpDTO> AsignarPuestoAsync(AsignarPuestoDTO dto);
        Task<ResponseSpDTO> FinalizarPuestoAsync(int asignacionId, FinalizarAsignacionDTO dto);
        Task<IEnumerable<HistorialPuestoResponseDTO>> ListarPuestosAsync(int empleadoId);

        Task<ResponseSpDTO> AsignarTurnoAsync(AsignarTurnoDTO dto);
        Task<ResponseSpDTO> FinalizarTurnoAsync(int asignacionId, FinalizarAsignacionDTO dto);
        Task<IEnumerable<AsignacionTurnoResponseDTO>> ListarTurnosAsync(int empleadoId);
    }
}
