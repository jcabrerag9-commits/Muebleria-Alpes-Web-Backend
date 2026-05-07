using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoDeduccion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/tipos-deduccion")]
    public class TiposDeduccionController : ControllerBase
    {
        private readonly ITipoDeduccionService _tipoDeduccionService;

        public TiposDeduccionController(ITipoDeduccionService tipoDeduccionService)
        {
            _tipoDeduccionService = tipoDeduccionService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var resultado = await _tipoDeduccionService.ListarAsync();

                return Ok(new
                {
                    mensaje = "Tipos de deducción obtenidos correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener tipos de deducción",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateTipoDeduccionDTO dto)
        {
            try
            {
                var resultado = await _tipoDeduccionService.CrearAsync(dto);

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
                    mensaje = "Tipo de deducción creado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear tipo de deducción",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateTipoDeduccionDTO dto)
        {
            try
            {
                var resultado = await _tipoDeduccionService.ActualizarAsync(id, dto);

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
                    mensaje = "Tipo de deducción actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar tipo de deducción",
                    detalle = ex.Message
                });
            }
        }
    }
}
