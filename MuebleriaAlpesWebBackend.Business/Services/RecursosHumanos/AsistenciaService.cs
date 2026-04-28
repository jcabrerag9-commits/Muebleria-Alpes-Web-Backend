using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Asistencia;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _repository;

        public AsistenciaService(IAsistenciaRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseSpDTO> RegistrarAsync(RegistrarAsistenciaDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();
            return await _repository.RegistrarAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarAsistenciaDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();
            return await _repository.ActualizarAsync(id, dto);
        }

        public Task<IEnumerable<AsistenciaResponseDTO>> ListarAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
            => _repository.ListarAsync(empleadoId, fechaInicio, fechaFin);
    }
}
