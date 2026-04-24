using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesClienteController : ControllerBase
    {
        private readonly IReportesClienteService _reportesClienteService;

        public ReportesClienteController(IReportesClienteService reportesClienteService)
        {
            _reportesClienteService = reportesClienteService;
        }

        [HttpGet("total-compras")]
        public async Task<IActionResult> TotalComprasCliente([FromQuery] int clienteId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var request = new ReporteClienteBaseRequest
                {
                    ClienteId = clienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesClienteService.TotalComprasClienteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("ltv")]
        public async Task<IActionResult> LtvCliente([FromQuery] int clienteId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var request = new ReporteClienteBaseRequest
                {
                    ClienteId = clienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesClienteService.LtvClienteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("ticket-promedio")]
        public async Task<IActionResult> TicketPromedioCliente([FromQuery] int clienteId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var request = new ReporteClienteBaseRequest
                {
                    ClienteId = clienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var response = await _reportesClienteService.TicketPromedioClienteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("compras-por-cliente")]
        public async Task<IActionResult> GenerarReporteComprasPorCliente([FromBody] GenerarReporteComprasClienteRequest request)
        {
            try
            {
                var response = await _reportesClienteService.GenerarReporteComprasPorClienteAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}