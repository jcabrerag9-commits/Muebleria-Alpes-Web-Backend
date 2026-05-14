using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IFacturacionService
    {
        Task<ApiResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request);
        Task<ApiResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request);
        Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId);
        Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId);
        Task<IEnumerable<FacturaDTO>> ObtenerTodasAsync(string? estado = null, int? clienteId = null, string? nit = null, System.Threading.CancellationToken ct = default);
        Task<FacturaDTO?> ObtenerDetallePorIdAsync(int facturaId, System.Threading.CancellationToken ct = default);
        Task<IEnumerable<OrdenPendienteDTO>> ObtenerOrdenesPendientesAsync(System.Threading.CancellationToken ct = default);
    }
}
