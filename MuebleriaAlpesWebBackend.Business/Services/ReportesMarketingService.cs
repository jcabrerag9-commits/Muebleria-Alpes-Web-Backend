using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ReportesMarketingService : IReportesMarketingService
    {
        private readonly IReportesMarketingRepository _reportesMarketingRepository;

        public ReportesMarketingService(IReportesMarketingRepository reportesMarketingRepository)
        {
            _reportesMarketingRepository = reportesMarketingRepository;
        }

        public async Task<LtvMarketingResponse> LtvClienteAsync(ReporteMarketingClienteRequest request)
        {
            return await _reportesMarketingRepository.LtvClienteAsync(request);
        }

        public async Task<TasaRetencionResponse> TasaRetencionAsync(ReporteMarketingRangoRequest request)
        {
            return await _reportesMarketingRepository.TasaRetencionAsync(request);
        }

        public async Task<TasaConversionResponse> TasaConversionAsync(ReporteMarketingRangoRequest request)
        {
            return await _reportesMarketingRepository.TasaConversionAsync(request);
        }

        public async Task<List<ReporteLtvItemResponse>> GenerarReporteLtvAsync(GenerarReporteMarketingRequest request)
        {
            return await _reportesMarketingRepository.GenerarReporteLtvAsync(request);
        }

        public async Task<List<ReporteActividadItemResponse>> GenerarReporteActividadAsync(GenerarReporteMarketingRequest request)
        {
            return await _reportesMarketingRepository.GenerarReporteActividadAsync(request);
        }

        public async Task<List<ReporteRetencionItemResponse>> GenerarReporteRetencionAsync(GenerarReporteMarketingRequest request)
        {
            return await _reportesMarketingRepository.GenerarReporteRetencionAsync(request);
        }

        public async Task<List<ReporteCohorteItemResponse>> GenerarReporteCohorteAsync(GenerarReporteMarketingRequest request)
        {
            return await _reportesMarketingRepository.GenerarReporteCohorteAsync(request);
        }

        public async Task<List<ReporteRemarketingItemResponse>> GenerarReporteRemarketingAsync(GenerarReporteRemarketingRequest request)
        {
            return await _reportesMarketingRepository.GenerarReporteRemarketingAsync(request);
        }
    }
}