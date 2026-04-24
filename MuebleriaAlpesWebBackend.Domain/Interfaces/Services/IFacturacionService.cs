using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IFacturacionService
    {
        Task<FacturacionResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request);
        Task<FacturacionResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request);
        Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId);
        Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId);
    }
}
