using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface IEvaluacionService
    {
        Task<ResponseSpDTO> CrearAsync(CrearEvaluacionDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarEvaluacionDTO dto);
        Task<EvaluacionResponseDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<EvaluacionResponseDTO>> ListarPorEmpleadoAsync(int empleadoId);
        Task<IEnumerable<EvaluacionResponseDTO>> ReporteAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<PromedioEvaluacionResponseDTO> ObtenerPromedioAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin);
    }
}
