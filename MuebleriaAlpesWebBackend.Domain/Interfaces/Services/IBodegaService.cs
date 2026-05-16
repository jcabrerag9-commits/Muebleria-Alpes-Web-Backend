using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IBodegaService
    {
        Task<InventarioResponse<int?>> CrearBodegaAsync(BodegaDTO bodega, int usuarioId);
        Task<InventarioResponse<bool>> ActualizarBodegaAsync(BodegaDTO bodega, int usuarioId);
        Task<InventarioResponse<bool>> CambiarEstadoBodegaAsync(int bodegaId, string nuevoEstado, string motivo, int usuarioId);
        Task<BodegaDTO?> ObtenerBodegaPorIdAsync(int bodegaId);
        Task<IEnumerable<BodegaDTO>> ListarBodegasAsync(bool soloActivas = false);
    }
}
