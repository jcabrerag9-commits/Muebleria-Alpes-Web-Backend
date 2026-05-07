using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoPago;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class TipoPagoService : ITipoPagoService
    {
        private readonly ITipoPagoRepository _tipoPagoRepository;

        public TipoPagoService(ITipoPagoRepository tipoPagoRepository)
        {
            _tipoPagoRepository = tipoPagoRepository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateTipoPagoDTO dto)
        {
            dto.Codigo = dto.Codigo.Trim().ToUpper();
            dto.Nombre = dto.Nombre.Trim();

            return await _tipoPagoRepository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTipoPagoDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();

            return await _tipoPagoRepository.ActualizarAsync(id, dto);
        }

        public async Task<IEnumerable<ResponseTipoPagoDTO>> ListarAsync()
        {
            return await _tipoPagoRepository.ListarAsync();
        }
    }
}
