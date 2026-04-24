using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IReportesCajaService
    {
        Task<TotalVentasCorteCajaResponse> TotalVentasCorteCajaAsync(CorteCajaBaseRequest request);

        Task<DiferenciaCorteCajaResponse> DiferenciaCorteCajaAsync(CorteCajaBaseRequest request);

        Task<List<ReporteCorteCajaItemResponse>> GenerarReporteCorteCajaAsync(GenerarReporteCorteCajaRequest request);
    }
}