using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Reportes
{
    public interface IReportesVentasService
    {
        Task<TotalVentasRangoResponse> TotalVentasRangoAsync(ReporteVentasRangoRequest request);

        Task<TotalVentasCiudadResponse> TotalVentasPorCiudadAsync(ReporteVentasCiudadRequest request);

        Task<ProductoMasVendidoResponse> ProductoMasVendidoRangoAsync(ReporteProductoMasVendidoRequest request);

        Task<TotalIngresosCanalResponse> TotalIngresosPorCanalAsync(ReporteIngresosCanalRequest request);

        Task<List<ReporteVentasDiariasItemResponse>> GenerarReporteVentasDiariasAsync(GenerarReporteVentasDiariasRequest request);

        Task<List<ReporteProductoMasVendidoItemResponse>> GenerarReporteProductoMasVendidoAsync(ReporteProductoMasVendidoRequest request);


        Task<List<FiltroCanalVentaResponse>> ListarCanalesVentaFiltroAsync();

        Task<List<FiltroCiudadResponse>> ListarCiudadesFiltroAsync();
    }
}