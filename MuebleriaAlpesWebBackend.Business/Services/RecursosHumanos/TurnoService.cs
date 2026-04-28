using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class TurnoService : ITurnoService
    {
        private readonly ITurnoRepository _turnoRepository;

        public TurnoService(ITurnoRepository turnoRepository)
        {
            _turnoRepository = turnoRepository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateTurnoDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();

            return await _turnoRepository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTurnoDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();

            return await _turnoRepository.ActualizarAsync(id, dto);
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoTurnoDTO dto)
        {
            dto.Estado = dto.Estado.Trim().ToUpper();

            return await _turnoRepository.CambiarEstadoAsync(id, dto);
        }

        public async Task<ResponseTurnoDTO?> ObtenerPorIdAsync(int id)
        {
            return await _turnoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ResponseTurnoDTO>> ListarAsync(bool soloActivos)
        {
            return await _turnoRepository.ListarAsync(soloActivos);
        }
    }
}
