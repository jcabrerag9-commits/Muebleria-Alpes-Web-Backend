using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina;
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
    public class NominaService : INominaService
    {
        private readonly INominaRepository _repository;

        public NominaService(INominaRepository repository)
        {
            _repository = repository;
        }

        public Task<ResponseSpDTO> CrearAsync(CrearNominaDTO dto)
        {
            dto.Periodo = dto.Periodo.Trim().ToUpper();
            return _repository.CrearAsync(dto);
        }

        public Task<ResponseSpDTO> CambiarEstadoAsync(int nominaId, CambiarEstadoNominaDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();
            return _repository.CambiarEstadoAsync(nominaId, dto);
        }

        public Task<NominaResponseDTO?> ObtenerPorIdAsync(int nominaId)
            => _repository.ObtenerPorIdAsync(nominaId);

        public Task<IEnumerable<NominaResponseDTO>> ListarAsync(string? estado)
        {
            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim().ToUpper();
            return _repository.ListarAsync(estado);
        }

        public Task<ResponseSpDTO> AgregarEmpleadoAsync(int nominaId, AgregarEmpleadoNominaDTO dto)
            => _repository.AgregarEmpleadoAsync(nominaId, dto);

        public Task<ResponseSpDTO> CalcularDetalleAsync(int detalleId, CalcularNominaDetalleDTO dto)
            => _repository.CalcularDetalleAsync(detalleId, dto);

        public Task<IEnumerable<NominaDetalleResponseDTO>> ListarDetalleAsync(int nominaId)
            => _repository.ListarDetalleAsync(nominaId);

        public Task<ResponseSpDTO> AgregarIngresoAsync(int detalleId, AgregarIngresoNominaDTO dto)
            => _repository.AgregarIngresoAsync(detalleId, dto);

        public Task<IEnumerable<NominaIngresoResponseDTO>> ListarIngresosAsync(int detalleId)
            => _repository.ListarIngresosAsync(detalleId);

        public Task<ResponseSpDTO> AgregarDeduccionAsync(int detalleId, AgregarDeduccionNominaDTO dto)
            => _repository.AgregarDeduccionAsync(detalleId, dto);

        public Task<IEnumerable<NominaDeduccionResponseDTO>> ListarDeduccionesAsync(int detalleId)
            => _repository.ListarDeduccionesAsync(detalleId);
    }
}
