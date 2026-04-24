using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
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
    }
}