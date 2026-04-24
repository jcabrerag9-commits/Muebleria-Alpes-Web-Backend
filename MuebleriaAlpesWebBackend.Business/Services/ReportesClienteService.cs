using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ReportesClienteService : IReportesClienteService
    {
        private readonly IReportesClienteRepository _reportesClienteRepository;

        public ReportesClienteService(IReportesClienteRepository reportesClienteRepository)
        {
            _reportesClienteRepository = reportesClienteRepository;
        }

        public async Task<TotalComprasClienteResponse> TotalComprasClienteAsync(ReporteClienteBaseRequest request)
        {
            return await _reportesClienteRepository.TotalComprasClienteAsync(request);
        }

        public async Task<LtvClienteResponse> LtvClienteAsync(ReporteClienteBaseRequest request)
        {
            return await _reportesClienteRepository.LtvClienteAsync(request);
        }

        public async Task<TicketPromedioClienteResponse> TicketPromedioClienteAsync(ReporteClienteBaseRequest request)
        {
            return await _reportesClienteRepository.TicketPromedioClienteAsync(request);
        }

        public async Task<List<ReporteComprasClienteItemResponse>> GenerarReporteComprasPorClienteAsync(GenerarReporteComprasClienteRequest request)
        {
            return await _reportesClienteRepository.GenerarReporteComprasPorClienteAsync(request);
        }
    }
}