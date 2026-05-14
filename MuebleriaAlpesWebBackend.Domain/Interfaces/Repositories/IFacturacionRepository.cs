using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IFacturacionRepository
    {
        Task<ApiResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request, System.Threading.CancellationToken ct = default);
        Task<ApiResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request, System.Threading.CancellationToken ct = default);
        Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId, System.Threading.CancellationToken ct = default);
        Task<bool> ActualizarEstadoFacturaAsync(int facturaId, string estado, System.Data.IDbTransaction transaction = null, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<FacturaDTO>> ObtenerTodasAsync(string? estado = null, int? clienteId = null, string? nit = null, System.Threading.CancellationToken ct = default);
        Task<FacturaDTO?> ObtenerDetallePorIdAsync(int facturaId, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<OrdenPendienteDTO>> ObtenerOrdenesPendientesAsync(System.Threading.CancellationToken ct = default);
    }
}
