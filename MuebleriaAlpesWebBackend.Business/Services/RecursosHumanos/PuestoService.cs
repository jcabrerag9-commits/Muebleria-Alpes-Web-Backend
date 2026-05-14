using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Puesto;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class PuestoService : IPuestoService
    {
        private readonly IPuestoRepository _puestoRepository;

        public PuestoService(IPuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreatePuestoDTO dto)
        {
            dto.Codigo = dto.Codigo.Trim().ToUpper();
            dto.Nombre = dto.Nombre.Trim();
            dto.Descripcion = dto.Descripcion?.Trim();

            return await _puestoRepository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdatePuestoDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();
            dto.Descripcion = dto.Descripcion?.Trim();

            return await _puestoRepository.ActualizarAsync(id, dto);
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoPuestoDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();

            return await _puestoRepository.CambiarEstadoAsync(id, dto);
        }

        public async Task<ResponsePuestoDTO?> ObtenerPorIdAsync(int id)
        {
            return await _puestoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ResponsePuestoDTO>> ListarAsync(bool soloActivos)
        {
            return await _puestoRepository.ListarAsync(soloActivos);
        }
    }
}
