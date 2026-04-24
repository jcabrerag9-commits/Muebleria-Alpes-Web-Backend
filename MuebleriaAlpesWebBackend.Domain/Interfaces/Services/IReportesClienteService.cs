using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IReportesClienteService
    {
        Task<TotalComprasClienteResponse> TotalComprasClienteAsync(ReporteClienteBaseRequest request);

        Task<LtvClienteResponse> LtvClienteAsync(ReporteClienteBaseRequest request);

        Task<TicketPromedioClienteResponse> TicketPromedioClienteAsync(ReporteClienteBaseRequest request);

        Task<List<ReporteComprasClienteItemResponse>> GenerarReporteComprasPorClienteAsync(GenerarReporteComprasClienteRequest request);
    }
}