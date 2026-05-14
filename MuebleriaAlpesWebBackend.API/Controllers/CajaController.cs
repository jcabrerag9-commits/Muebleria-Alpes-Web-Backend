using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Caja;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly ICajaService _cajaService;

        public CajaController(ICajaService cajaService)
        {
            _cajaService = cajaService;
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaRequestDto request)
        {
            try
            {
                var resultado = await _cajaService.AbrirCajaAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al abrir la caja", detalle = ex.Message });
            }
        }

        [HttpPut("cerrar")]
        public async Task<IActionResult> CerrarCaja([FromBody] CerrarCajaRequestDto request)
        {
            try
            {
                var resultado = await _cajaService.CerrarCajaAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al cerrar la caja", detalle = ex.Message });
            }
        }

        [HttpPut("conciliar")]
        public async Task<IActionResult> ConciliarCaja([FromBody] ConciliarCajaRequestDto request)
        {
            try
            {
                var resultado = await _cajaService.ConciliarCajaAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al conciliar la caja", detalle = ex.Message });
            }
        }
    }
}
