using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class FacturacionService : IFacturacionService
    {
        private readonly IFacturacionRepository _facturacionRepository;

        public FacturacionService(IFacturacionRepository facturacionRepository)
        {
            _facturacionRepository = facturacionRepository;
        }

        public async Task<ApiResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request)
        {
            // Validaciones mínimas
            // Validaciones mínimas: PagoId puede ser 0 si se factura antes de pagar
            if (request.OrdenId <= 0)
            {
                return new ApiResponse<int?>
                {
                    Success = false,
                    Message = "ID de orden inválido.",
                    Data = null
                };
            }

            return await _facturacionRepository.GenerarFacturaAsync(request);
        }

        public async Task<ApiResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request)
        {
            if (request.FacturaId <= 0 || string.IsNullOrWhiteSpace(request.Motivo))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "ID de factura y motivo son requeridos.",
                    Data = false
                };
            }

            return await _facturacionRepository.AnularFacturaAsync(request);
        }

        public async Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId)
        {
            return await _facturacionRepository.ObtenerFacturaPorIdAsync(facturaId);
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId)
        {
            return await _facturacionRepository.ObtenerFacturasPorClienteAsync(clienteId);
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerTodasAsync(string? estado = null, int? clienteId = null, string? nit = null, System.Threading.CancellationToken ct = default)
        {
            return await _facturacionRepository.ObtenerTodasAsync(estado, clienteId, nit, ct);
        }

        public async Task<FacturaDTO?> ObtenerDetallePorIdAsync(int facturaId, System.Threading.CancellationToken ct = default)
        {
            return await _facturacionRepository.ObtenerDetallePorIdAsync(facturaId, ct);
        }

        public async Task<IEnumerable<OrdenPendienteDTO>> ObtenerOrdenesPendientesAsync(System.Threading.CancellationToken ct = default)
        {
            return await _facturacionRepository.ObtenerOrdenesPendientesAsync(ct);
        }
    }
}
