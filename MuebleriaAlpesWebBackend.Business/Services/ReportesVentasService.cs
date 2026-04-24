using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ReportesVentasService : IReportesVentasService
    {
        private readonly IReportesVentasRepository _reportesVentasRepository;

        public ReportesVentasService(IReportesVentasRepository reportesVentasRepository)
        {
            _reportesVentasRepository = reportesVentasRepository;
        }

        public async Task<TotalVentasRangoResponse> TotalVentasRangoAsync(ReporteVentasRangoRequest request)
        {
            return await _reportesVentasRepository.TotalVentasRangoAsync(request);
        }

        public async Task<TotalVentasCiudadResponse> TotalVentasPorCiudadAsync(ReporteVentasCiudadRequest request)
        {
            return await _reportesVentasRepository.TotalVentasPorCiudadAsync(request);
        }

        public async Task<ProductoMasVendidoResponse> ProductoMasVendidoRangoAsync(ReporteProductoMasVendidoRequest request)
        {
            return await _reportesVentasRepository.ProductoMasVendidoRangoAsync(request);
        }

        public async Task<TotalIngresosCanalResponse> TotalIngresosPorCanalAsync(ReporteIngresosCanalRequest request)
        {
            return await _reportesVentasRepository.TotalIngresosPorCanalAsync(request);
        }

        public async Task<List<ReporteVentasDiariasItemResponse>> GenerarReporteVentasDiariasAsync(GenerarReporteVentasDiariasRequest request)
        {
            return await _reportesVentasRepository.GenerarReporteVentasDiariasAsync(request);
        }

        public async Task<List<ReporteProductoMasVendidoItemResponse>> GenerarReporteProductoMasVendidoAsync(ReporteProductoMasVendidoRequest request)
        {
            return await _reportesVentasRepository.GenerarReporteProductoMasVendidoAsync(request);
        }
    }
}