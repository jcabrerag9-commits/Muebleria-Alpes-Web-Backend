using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class AsignacionEmpleadoService : IAsignacionEmpleadoService
    {
        private readonly IAsignacionEmpleadoRepository _repository;

        public AsignacionEmpleadoService(IAsignacionEmpleadoRepository repository)
        {
            _repository = repository;
        }

        public Task<ResponseSpDTO> AsignarDepartamentoAsync(AsignarDepartamentoDTO dto)
            => _repository.AsignarDepartamentoAsync(dto);

        public Task<ResponseSpDTO> FinalizarDepartamentoAsync(int asignacionId, FinalizarAsignacionDTO dto)
            => _repository.FinalizarDepartamentoAsync(asignacionId, dto);

        public Task<IEnumerable<AsignarDepartamentoResponseDTO>> ListarDepartamentosAsync(int empleadoId)
            => _repository.ListarDepartamentosAsync(empleadoId);

        public Task<ResponseSpDTO> AsignarPuestoAsync(AsignarPuestoDTO dto)
            => _repository.AsignarPuestoAsync(dto);

        public Task<ResponseSpDTO> FinalizarPuestoAsync(int asignacionId, FinalizarAsignacionDTO dto)
            => _repository.FinalizarPuestoAsync(asignacionId, dto);

        public Task<IEnumerable<HistorialPuestoResponseDTO>> ListarPuestosAsync(int empleadoId)
            => _repository.ListarPuestosAsync(empleadoId);

        public Task<ResponseSpDTO> AsignarTurnoAsync(AsignarTurnoDTO dto)
            => _repository.AsignarTurnoAsync(dto);

        public Task<ResponseSpDTO> FinalizarTurnoAsync(int asignacionId, FinalizarAsignacionDTO dto)
            => _repository.FinalizarTurnoAsync(asignacionId, dto);

        public Task<IEnumerable<AsignacionTurnoResponseDTO>> ListarTurnosAsync(int empleadoId)
            => _repository.ListarTurnosAsync(empleadoId);
    }
}
