using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public EmpleadoService(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateEmpleadoDTO dto)
        {
            dto.Codigo = dto.Codigo.Trim().ToUpper();
            dto.NumeroDocumento = dto.NumeroDocumento.Trim();
            dto.PrimerNombre = dto.PrimerNombre.Trim();
            dto.SegundoNombre = dto.SegundoNombre?.Trim();
            dto.PrimerApellido = dto.PrimerApellido.Trim();
            dto.SegundoApellido = dto.SegundoApellido?.Trim();
            dto.Email = dto.Email.Trim().ToLower();
            dto.Telefono = dto.Telefono?.Trim();

            return await _empleadoRepository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateEmpleadoDTO dto)
        {
            dto.Email = dto.Email.Trim().ToLower();
            dto.Telefono = dto.Telefono?.Trim();

            return await _empleadoRepository.ActualizarAsync(id, dto);
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoEmpleadoDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();
            dto.Motivo = dto.Motivo?.Trim();

            return await _empleadoRepository.CambiarEstadoAsync(id, dto);
        }

        public async Task<ResponseEmpleadoDTO?> ObtenerPorIdAsync(int id)
        {
            return await _empleadoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ResponseEmpleadoDTO>> ListarAsync(string? estado)
        {
            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim().ToUpper();

            return await _empleadoRepository.ListarAsync(estado);
        }
    }
}
