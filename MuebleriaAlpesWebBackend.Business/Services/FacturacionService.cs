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

        public async Task<FacturacionResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request)
        {
            // Validaciones mínimas
            if (request.OrdenId <= 0 || request.PagoId <= 0)
            {
                return new FacturacionResponse<int?>
                {
                    Resultado = "ERROR",
                    Mensaje = "Datos de orden o pago inválidos.",
                    Data = null
                };
            }

            return await _facturacionRepository.GenerarFacturaAsync(request);
        }

        public async Task<FacturacionResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request)
        {
            if (request.FacturaId <= 0 || string.IsNullOrWhiteSpace(request.Motivo))
            {
                return new FacturacionResponse<bool>
                {
                    Resultado = "ERROR",
                    Mensaje = "ID de factura y motivo son requeridos.",
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
    }
}
