using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly IBodegaRepository _bodegaRepository;

        public BodegaService(IBodegaRepository bodegaRepository)
        {
            _bodegaRepository = bodegaRepository;
        }

        public async Task<InventarioResponse<int?>> CrearBodegaAsync(BodegaDTO bodega, int usuarioId)
        {
            return await _bodegaRepository.CrearBodegaAsync(bodega, usuarioId);
        }

        public async Task<InventarioResponse<bool>> ActualizarBodegaAsync(BodegaDTO bodega, int usuarioId)
        {
            return await _bodegaRepository.ActualizarBodegaAsync(bodega, usuarioId);
        }

        public async Task<InventarioResponse<bool>> CambiarEstadoBodegaAsync(int bodegaId, string nuevoEstado, string motivo, int usuarioId)
        {
            return await _bodegaRepository.CambiarEstadoBodegaAsync(bodegaId, nuevoEstado, motivo, usuarioId);
        }

        public async Task<BodegaDTO?> ObtenerBodegaPorIdAsync(int bodegaId)
        {
            return await _bodegaRepository.ObtenerBodegaPorIdAsync(bodegaId);
        }

        public async Task<IEnumerable<BodegaDTO>> ListarBodegasAsync(bool soloActivas = false)
        {
            return await _bodegaRepository.ListarBodegasAsync(soloActivas);
        }
    }
}
