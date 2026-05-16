using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        [HttpPost("ejecucion")]
        public async Task<IActionResult> RegistrarEjecucionReporte([FromBody] RegistrarEjecucionReporteRequestDto request)
        {
            try
            {
                var resultado = await _reportesService.RegistrarEjecucionReporteAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al registrar la ejecución del reporte", detalle = ex.Message });
            }
        }
    }
}
