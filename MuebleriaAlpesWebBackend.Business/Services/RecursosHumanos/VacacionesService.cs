using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones;
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
    public class VacacionesService : IVacacionesService
    {
        private readonly IVacacionRepository _repository;

        public VacacionesService(IVacacionRepository repository)
        {
            _repository = repository;
        }

        public Task<ResponseSpDTO> SolicitarAsync(SolicitarVacacionesDTO dto)
            => _repository.SolicitarAsync(dto);

        public Task<ResponseSpDTO> AprobarAsync(int id, CambiarEstadoVacacionesDTO dto)
            => _repository.AprobarAsync(id, dto);

        public async Task<ResponseSpDTO> RechazarAsync(int id, CambiarEstadoVacacionesDTO dto)
        {
            dto.Motivo = dto.Motivo?.Trim();
            return await _repository.RechazarAsync(id, dto);
        }

        public async Task<ResponseSpDTO> CancelarAsync(int id, CambiarEstadoVacacionesDTO dto)
        {
            dto.Motivo = dto.Motivo?.Trim();
            return await _repository.CancelarAsync(id, dto);
        }

        public async Task<IEnumerable<VacacionResponseDTO>> ListarAsync(int empleadoId, string? estado)
        {
            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim().ToUpper();
            return await _repository.ListarAsync(empleadoId, estado);
        }
    }
}
