using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Reportes
{
    public interface IReportesCajaService
    {
        Task<TotalVentasCorteCajaResponse> TotalVentasCorteCajaAsync(CorteCajaBaseRequest request);

        Task<DiferenciaCorteCajaResponse> DiferenciaCorteCajaAsync(CorteCajaBaseRequest request);

        Task<List<ReporteCorteCajaItemResponse>> GenerarReporteCorteCajaAsync(GenerarReporteCorteCajaRequest request);


        Task<List<FiltroCorteCajaResponse>> ListarCortesCajaFiltroAsync(string? estado);
    }
}