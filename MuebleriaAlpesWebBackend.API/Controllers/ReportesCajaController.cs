using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesCajaController : ControllerBase
    {
        private readonly IReportesCajaService _reportesCajaService;

        public ReportesCajaController(IReportesCajaService reportesCajaService)
        {
            _reportesCajaService = reportesCajaService;
        }

        [HttpGet("total-ventas-corte")]
        public async Task<IActionResult> TotalVentasCorteCaja([FromQuery] int corteCajaId)
        {
            try
            {
                var request = new CorteCajaBaseRequest
                {
                    CorteCajaId = corteCajaId
                };

                var response = await _reportesCajaService.TotalVentasCorteCajaAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("diferencia-corte")]
        public async Task<IActionResult> DiferenciaCorteCaja([FromQuery] int corteCajaId)
        {
            try
            {
                var request = new CorteCajaBaseRequest
                {
                    CorteCajaId = corteCajaId
                };

                var response = await _reportesCajaService.DiferenciaCorteCajaAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reporte-corte-caja")]
        public async Task<IActionResult> GenerarReporteCorteCaja([FromBody] GenerarReporteCorteCajaRequest request)
        {
            try
            {
                var response = await _reportesCajaService.GenerarReporteCorteCajaAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}