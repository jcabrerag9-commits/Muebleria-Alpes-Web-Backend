using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IReportesVentasRepository
    {
        Task<TotalVentasRangoResponse> TotalVentasRangoAsync(ReporteVentasRangoRequest request);

        Task<TotalVentasCiudadResponse> TotalVentasPorCiudadAsync(ReporteVentasCiudadRequest request);

        Task<ProductoMasVendidoResponse> ProductoMasVendidoRangoAsync(ReporteProductoMasVendidoRequest request);

        Task<TotalIngresosCanalResponse> TotalIngresosPorCanalAsync(ReporteIngresosCanalRequest request);

        Task<List<ReporteVentasDiariasItemResponse>> GenerarReporteVentasDiariasAsync(GenerarReporteVentasDiariasRequest request);

        Task<List<ReporteProductoMasVendidoItemResponse>> GenerarReporteProductoMasVendidoAsync(ReporteProductoMasVendidoRequest request);
    }
}