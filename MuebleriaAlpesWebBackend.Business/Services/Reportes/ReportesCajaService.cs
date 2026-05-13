using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Reportes;

namespace MuebleriaAlpesWebBackend.Business.Services.Reportes
{
    public class ReportesCajaService : IReportesCajaService
    {
        private readonly IReportesCajaRepository _reportesCajaRepository;

        public ReportesCajaService(IReportesCajaRepository reportesCajaRepository)
        {
            _reportesCajaRepository = reportesCajaRepository;
        }

        public async Task<TotalVentasCorteCajaResponse> TotalVentasCorteCajaAsync(CorteCajaBaseRequest request)
        {
            return await _reportesCajaRepository.TotalVentasCorteCajaAsync(request);
        }

        public async Task<DiferenciaCorteCajaResponse> DiferenciaCorteCajaAsync(CorteCajaBaseRequest request)
        {
            return await _reportesCajaRepository.DiferenciaCorteCajaAsync(request);
        }

        public async Task<List<ReporteCorteCajaItemResponse>> GenerarReporteCorteCajaAsync(GenerarReporteCorteCajaRequest request)
        {
            return await _reportesCajaRepository.GenerarReporteCorteCajaAsync(request);
        }


        public async Task<List<FiltroCorteCajaResponse>> ListarCortesCajaFiltroAsync(string? estado)
        {
            return await _reportesCajaRepository.ListarCortesCajaFiltroAsync(estado);
        }
    }
}