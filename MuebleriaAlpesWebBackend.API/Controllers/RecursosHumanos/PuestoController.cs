using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Puesto;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/puestos")]
    public class PuestoController : ControllerBase
    {
        private readonly IPuestoService _puestoService;

        public PuestoController(IPuestoService puestoService)
        {
            _puestoService = puestoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true)
        {
            try
            {
                var resultado = await _puestoService.ListarAsync(soloActivos);

                return Ok(new
                {
                    mensaje = "Puestos obtenidos correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener puestos",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _puestoService.ObtenerPorIdAsync(id);

                if (resultado == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Puesto no encontrado"
                    });
                }

                return Ok(new
                {
                    mensaje = "Puesto obtenido correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener puesto",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreatePuestoDTO dto)
        {
            try
            {
                var resultado = await _puestoService.CrearAsync(dto);

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
                    mensaje = "Puesto creado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear puesto",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdatePuestoDTO dto)
        {
            try
            {
                var resultado = await _puestoService.ActualizarAsync(id, dto);

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
                    mensaje = "Puesto actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar puesto",
                    detalle = ex.Message
                });
            }
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoPuestoDTO dto)
        {
            try
            {
                var resultado = await _puestoService.CambiarEstadoAsync(id, dto);

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
                    mensaje = "Estado del puesto actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al cambiar estado del puesto",
                    detalle = ex.Message
                });
            }
        }
    }
}
