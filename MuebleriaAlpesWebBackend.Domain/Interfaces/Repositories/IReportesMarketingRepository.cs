using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IReportesMarketingRepository
    {
        Task<LtvMarketingResponse> LtvClienteAsync(ReporteMarketingClienteRequest request);

        Task<TasaRetencionResponse> TasaRetencionAsync(ReporteMarketingRangoRequest request);

        Task<TasaConversionResponse> TasaConversionAsync(ReporteMarketingRangoRequest request);

        Task<List<ReporteLtvItemResponse>> GenerarReporteLtvAsync(GenerarReporteMarketingRequest request);

        Task<List<ReporteActividadItemResponse>> GenerarReporteActividadAsync(GenerarReporteMarketingRequest request);

        Task<List<ReporteRetencionItemResponse>> GenerarReporteRetencionAsync(GenerarReporteMarketingRequest request);

        Task<List<ReporteCohorteItemResponse>> GenerarReporteCohorteAsync(GenerarReporteMarketingRequest request);

        Task<List<ReporteRemarketingItemResponse>> GenerarReporteRemarketingAsync(GenerarReporteRemarketingRequest request);
    }
}