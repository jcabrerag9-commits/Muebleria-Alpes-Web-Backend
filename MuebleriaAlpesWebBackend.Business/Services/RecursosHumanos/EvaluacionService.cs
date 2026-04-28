using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class EvaluacionService : IEvaluacionService
    {
        private readonly IEvaluacionRepository _repository;

        public EvaluacionService(IEvaluacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CrearEvaluacionDTO dto)
        {
            dto.Comentarios = dto.Comentarios?.Trim();

            return await _repository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarEvaluacionDTO dto)
        {
            dto.Comentarios = dto.Comentarios?.Trim();

            return await _repository.ActualizarAsync(id, dto);
        }

        public Task<EvaluacionResponseDTO> ObtenerPorIdAsync(int id)
        {
            return _repository.ObtenerPorIdAsync(id);
        }

        public Task<IEnumerable<EvaluacionResponseDTO>> ListarPorEmpleadoAsync(int empleadoId)
        {
            return _repository.ListarPorEmpleadoAsync(empleadoId);
        }

        public Task<IEnumerable<EvaluacionResponseDTO>> ReporteAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return _repository.ReporteAsync(fechaInicio, fechaFin);
        }

        public Task<PromedioEvaluacionResponseDTO> ObtenerPromedioAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            return _repository.ObtenerPromedioAsync(empleadoId, fechaInicio, fechaFin);
        }
    }
}
