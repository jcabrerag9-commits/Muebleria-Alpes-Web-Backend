using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/turnos")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoService _turnoService;

        public TurnoController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true)
        {
            try
            {
                var resultado = await _turnoService.ListarAsync(soloActivos);

                return Ok(new
                {
                    mensaje = "Turnos obtenidos correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener turnos",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _turnoService.ObtenerPorIdAsync(id);

                if (resultado == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Turno no encontrado"
                    });
                }

                return Ok(new
                {
                    mensaje = "Turno obtenido correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener turno",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateTurnoDTO dto)
        {
            try
            {
                var resultado = await _turnoService.CrearAsync(dto);

                if (resultado.Resultado == "ERROR")
                {
                    return BadRequest(new
                    {
                        mensaje = resultado.Mensaje,
                        resultado
                    });
                }

                return Ok(new
                {
                    mensaje = "Turno creado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear turno",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateTurnoDTO dto)
        {
            try
            {
                var resultado = await _turnoService.ActualizarAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                {
                    return BadRequest(new
                    {
                        mensaje = resultado.Mensaje,
                        resultado
                    });
                }

                return Ok(new
                {
                    mensaje = "Turno actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar turno",
                    detalle = ex.Message
                });
            }
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoTurnoDTO dto)
        {
            try
            {
                var resultado = await _turnoService.CambiarEstadoAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                {
                    return BadRequest(new
                    {
                        mensaje = resultado.Mensaje,
                        resultado
                    });
                }

                return Ok(new
                {
                    mensaje = "Estado del turno actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al cambiar estado del turno",
                    detalle = ex.Message
                });
            }
        }
    }
}
