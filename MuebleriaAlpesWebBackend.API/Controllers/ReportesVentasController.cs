using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesVentasController : ControllerBase
    {
        private readonly IReportesVentasService _reportesVentasService;

        public ReportesVentasController(IReportesVentasService reportesVentasService)
        {
            _reportesVentasService = reportesVentasService;
        }

        [HttpGet("total-rango")]
        public async Task<IActionResult> TotalVentasRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var request = new ReporteVentasRangoRequest
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesVentasService.TotalVentasRangoAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("total-por-ciudad")]
        public async Task<IActionResult> TotalVentasPorCiudad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int ciudadId)
        {
            try
            {
                var request = new ReporteVentasCiudadRequest
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    CiudadId = ciudadId
                };

                var response = await _reportesVentasService.TotalVentasPorCiudadAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("producto-mas-vendido")]
        public async Task<IActionResult> ProductoMasVendidoRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int? ciudadId)
        {
            try
            {
                var request = new ReporteProductoMasVendidoRequest
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    CiudadId = ciudadId
                };

                var response = await _reportesVentasService.ProductoMasVendidoRangoAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("ingresos-por-canal")]
        public async Task<IActionResult> TotalIngresosPorCanal([FromQuery] int canalVentaId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var request = new ReporteIngresosCanalRequest
                {
                    CanalVentaId = canalVentaId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesVentasService.TotalIngresosPorCanalAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ventas-diarias")]
        public async Task<IActionResult> GenerarReporteVentasDiarias([FromBody] GenerarReporteVentasDiariasRequest request)
        {
            try
            {
                var response = await _reportesVentasService.GenerarReporteVentasDiariasAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("producto-mas-vendido-reporte")]
        public async Task<IActionResult> GenerarReporteProductoMasVendido([FromBody] ReporteProductoMasVendidoRequest request)
        {
            try
            {
                var response = await _reportesVentasService.GenerarReporteProductoMasVendidoAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}