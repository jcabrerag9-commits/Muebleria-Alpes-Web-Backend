using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Ventas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/ventas")]
    public class VentasController : ControllerBase
    {
        private readonly IVentasService _ventasService;

        public VentasController(IVentasService ventasService)
        {
            _ventasService = ventasService;
        }

        [HttpPost("orden")]
        public async Task<IActionResult> CrearOrdenCompleta([FromBody] CrearOrdenRequestDto request)
        {
            try
            {
                var resultado = await _ventasService.CrearOrdenCompletaAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al crear la orden", detalle = ex.Message });
            }
        }

        [HttpPut("orden/estado")]
        public async Task<IActionResult> ActualizarEstadoOrden([FromBody] ActualizarEstadoOrdenRequestDto request)
        {
            try
            {
                var resultado = await _ventasService.ActualizarEstadoOrdenAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al actualizar estado", detalle = ex.Message });
            }
        }

        [HttpPost("orden/cancelar")]
        public async Task<IActionResult> CancelarOrden([FromBody] CancelarOrdenRequestDto request)
        {
            try
            {
                var resultado = await _ventasService.CancelarOrdenAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al cancelar la orden", detalle = ex.Message });
            }
        }

        [HttpPost("orden/promocion")]
        public async Task<IActionResult> AplicarPromocion([FromBody] AplicarPromocionRequestDto request)
        {
            try
            {
                var resultado = await _ventasService.AplicarPromocionAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al aplicar promoción", detalle = ex.Message });
            }
        }

        [HttpGet("orden/{ordenId}/totales")]
        public async Task<IActionResult> CalcularTotalesOrden(int ordenId)
        {
            try
            {
                var resultado = await _ventasService.CalcularTotalesOrdenAsync(ordenId);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al calcular totales", detalle = ex.Message });
            }
        }
    }
}
