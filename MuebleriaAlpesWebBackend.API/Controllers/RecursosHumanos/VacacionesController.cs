using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/vacaciones")]
    public class VacacionesController : ControllerBase
    {
        private readonly IVacacionesService _service;

        public VacacionesController(IVacacionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] int empleadoId, [FromQuery] string? estado = null)
        {
            try
            {
                var resultado = await _service.ListarAsync(empleadoId, estado);

                return Ok(new
                {
                    mensaje = "Vacaciones obtenidas correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener vacaciones",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar([FromBody] SolicitarVacacionesDTO dto)
        {
            try
            {
                var resultado = await _service.SolicitarAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Solicitud de vacaciones creada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al solicitar vacaciones", detalle = ex.Message });
            }
        }

        [HttpPatch("{id:int}/aprobar")]
        public async Task<IActionResult> Aprobar(int id, [FromBody] CambiarEstadoVacacionesDTO dto)
        {
            try
            {
                var resultado = await _service.AprobarAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Vacaciones aprobadas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al aprobar vacaciones", detalle = ex.Message });
            }
        }

        [HttpPatch("{id:int}/rechazar")]
        public async Task<IActionResult> Rechazar(int id, [FromBody] CambiarEstadoVacacionesDTO dto)
        {
            try
            {
                var resultado = await _service.RechazarAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Vacaciones rechazadas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al rechazar vacaciones", detalle = ex.Message });
            }
        }

        [HttpPatch("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] CambiarEstadoVacacionesDTO dto)
        {
            try
            {
                var resultado = await _service.CancelarAsync(id, dto);
                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Vacaciones canceladas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cancelar vacaciones", detalle = ex.Message });
            }
        }
    }
}
