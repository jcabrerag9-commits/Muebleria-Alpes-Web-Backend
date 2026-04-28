using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/empleados")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] string? estado = null)
        {
            try
            {
                var resultado = await _empleadoService.ListarAsync(estado);

                return Ok(new
                {
                    mensaje = "Empleados obtenidos correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener empleados",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _empleadoService.ObtenerPorIdAsync(id);

                if (resultado == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Empleado no encontrado"
                    });
                }

                return Ok(new
                {
                    mensaje = "Empleado obtenido correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener empleado",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateEmpleadoDTO dto)
        {
            try
            {
                var resultado = await _empleadoService.CrearAsync(dto);

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
                    mensaje = "Empleado creado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear empleado",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateEmpleadoDTO dto)
        {
            try
            {
                var resultado = await _empleadoService.ActualizarAsync(id, dto);

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
                    mensaje = "Empleado actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar empleado",
                    detalle = ex.Message
                });
            }
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoEmpleadoDTO dto)
        {
            try
            {
                var resultado = await _empleadoService.CambiarEstadoAsync(id, dto);

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
                    mensaje = "Estado del empleado actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al cambiar estado del empleado",
                    detalle = ex.Message
                });
            }
        }
    }
}
