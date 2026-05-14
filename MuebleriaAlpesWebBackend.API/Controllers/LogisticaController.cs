using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Logistica;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogisticaController : ControllerBase
    {
        private readonly ILogisticaService _logisticaService;

        public LogisticaController(ILogisticaService logisticaService)
        {
            _logisticaService = logisticaService;
        }

        [HttpPost("envio")]
        public async Task<IActionResult> CrearEnvio([FromBody] CrearEnvioRequestDto request)
        {
            try
            {
                var resultado = await _logisticaService.CrearEnvioAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al crear el envío", detalle = ex.Message });
            }
        }

        [HttpPut("envio/estado")]
        public async Task<IActionResult> ActualizarEstadoEnvio([FromBody] ActualizarEstadoEnvioRequestDto request)
        {
            try
            {
                var resultado = await _logisticaService.ActualizarEstadoEnvioAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al actualizar el estado del envío", detalle = ex.Message });
            }
        }
    }
}
