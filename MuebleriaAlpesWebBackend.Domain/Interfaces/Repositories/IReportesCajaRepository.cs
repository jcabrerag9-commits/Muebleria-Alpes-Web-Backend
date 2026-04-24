using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IReportesCajaRepository
    {
        Task<TotalVentasCorteCajaResponse> TotalVentasCorteCajaAsync(CorteCajaBaseRequest request);

        Task<DiferenciaCorteCajaResponse> DiferenciaCorteCajaAsync(CorteCajaBaseRequest request);

        Task<List<ReporteCorteCajaItemResponse>> GenerarReporteCorteCajaAsync(GenerarReporteCorteCajaRequest request);
    }
}