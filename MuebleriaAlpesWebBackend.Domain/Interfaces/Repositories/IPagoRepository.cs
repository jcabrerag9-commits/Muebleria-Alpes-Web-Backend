using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IPagoRepository
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, System.Data.IDbTransaction transaction = null, System.Threading.CancellationToken ct = default);

        Task<decimal> ObtenerSaldoPendienteOracleAsync(int facturaId, IDbTransaction transaction = null, System.Threading.CancellationToken ct = default);
        Task<PagoDTO?> ObtenerPorIdAsync(int id, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<PagoDTO>> ObtenerTodosAsync(System.Threading.CancellationToken ct = default);
        Task<IEnumerable<PagoDTO>> ObtenerPorFacturaIdAsync(int facturaId, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<OrdenPendientePagoDTO>> ObtenerOrdenesPendientesAsync(System.Threading.CancellationToken ct = default);
        Task<ApiResponse<bool>> AnularPagoAsync(int id, string motivo, int usuarioId, CancellationToken ct = default);
    }
}
