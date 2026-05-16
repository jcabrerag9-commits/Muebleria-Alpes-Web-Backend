using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IPagoService
    {
        Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, System.Threading.CancellationToken ct = default);
        Task<PagoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<PagoDTO>> ObtenerTodosAsync();
        Task<IEnumerable<PagoDTO>> ObtenerPorFacturaIdAsync(int facturaId);
        Task<IEnumerable<OrdenPendientePagoDTO>> ObtenerOrdenesPendientesAsync();
        Task<ApiResponse<bool>> AnularPagoAsync(int id, string motivo, int usuarioId, System.Threading.CancellationToken ct = default);
    }
}
