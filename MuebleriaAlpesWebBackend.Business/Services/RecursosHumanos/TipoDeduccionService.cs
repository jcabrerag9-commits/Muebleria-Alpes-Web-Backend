using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoDeduccion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class TipoDeduccionService : ITipoDeduccionService
    {
        private readonly ITipoDeduccionRepository _tipoDeduccionRepository;

        public TipoDeduccionService(ITipoDeduccionRepository tipoDeduccionRepository)
        {
            _tipoDeduccionRepository = tipoDeduccionRepository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateTipoDeduccionDTO dto)
        {
            dto.Codigo = dto.Codigo.Trim().ToUpper();
            dto.Nombre = dto.Nombre.Trim();

            return await _tipoDeduccionRepository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTipoDeduccionDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();

            return await _tipoDeduccionRepository.ActualizarAsync(id, dto);
        }

        public async Task<IEnumerable<ResponseTipoDeduccionDTO>> ListarAsync()
        {
            return await _tipoDeduccionRepository.ListarAsync();
        }
    }
}
