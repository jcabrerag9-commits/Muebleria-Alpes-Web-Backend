using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesMarketingController : ControllerBase
    {
        private readonly IReportesMarketingService _reportesMarketingService;

        public ReportesMarketingController(IReportesMarketingService reportesMarketingService)
        {
            _reportesMarketingService = reportesMarketingService;
        }

        [HttpGet("ltv-cliente")]
        public async Task<IActionResult> LtvCliente([FromQuery] int clienteId, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var request = new ReporteMarketingClienteRequest
                {
                    ClienteId = clienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesMarketingService.LtvClienteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tasa-retencion")]
        public async Task<IActionResult> TasaRetencion([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var request = new ReporteMarketingRangoRequest
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesMarketingService.TasaRetencionAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tasa-conversion")]
        public async Task<IActionResult> TasaConversion([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var request = new ReporteMarketingRangoRequest
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesMarketingService.TasaConversionAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-ltv")]
        public async Task<IActionResult> GenerarReporteLtv([FromBody] GenerarReporteMarketingRequest request)
        {
            try
            {
                var response = await _reportesMarketingService.GenerarReporteLtvAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-actividad")]
        public async Task<IActionResult> GenerarReporteActividad([FromBody] GenerarReporteMarketingRequest request)
        {
            try
            {
                var response = await _reportesMarketingService.GenerarReporteActividadAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-retencion")]
        public async Task<IActionResult> GenerarReporteRetencion([FromBody] GenerarReporteMarketingRequest request)
        {
            try
            {
                var response = await _reportesMarketingService.GenerarReporteRetencionAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-cohorte")]
        public async Task<IActionResult> GenerarReporteCohorte([FromBody] GenerarReporteMarketingRequest request)
        {
            try
            {
                var response = await _reportesMarketingService.GenerarReporteCohorteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-remarketing")]
        public async Task<IActionResult> GenerarReporteRemarketing([FromBody] GenerarReporteRemarketingRequest request)
        {
            try
            {
                var response = await _reportesMarketingService.GenerarReporteRemarketingAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}