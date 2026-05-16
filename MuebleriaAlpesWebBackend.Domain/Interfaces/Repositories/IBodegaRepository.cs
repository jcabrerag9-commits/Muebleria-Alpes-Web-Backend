using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IBodegaRepository
    {
        Task<InventarioResponse<int?>> CrearBodegaAsync(BodegaDTO bodega, int usuarioId, CancellationToken ct = default);
        Task<InventarioResponse<bool>> ActualizarBodegaAsync(BodegaDTO bodega, int usuarioId, CancellationToken ct = default);
        Task<InventarioResponse<bool>> CambiarEstadoBodegaAsync(int bodegaId, string nuevoEstado, string motivo, int usuarioId, CancellationToken ct = default);
        Task<BodegaDTO?> ObtenerBodegaPorIdAsync(int bodegaId, CancellationToken ct = default);
        Task<IEnumerable<BodegaDTO>> ListarBodegasAsync(bool soloActivas = false, CancellationToken ct = default);
    }
}
