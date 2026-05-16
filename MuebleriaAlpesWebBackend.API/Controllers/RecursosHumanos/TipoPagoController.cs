using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoPago;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/tipos-pago")]
    public class TiposPagoController : ControllerBase
    {
        private readonly ITipoPagoService _tipoPagoService;

        public TiposPagoController(ITipoPagoService tipoPagoService)
        {
            _tipoPagoService = tipoPagoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var resultado = await _tipoPagoService.ListarAsync();

                return Ok(new
                {
                    mensaje = "Tipos de pago obtenidos correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener tipos de pago",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateTipoPagoDTO dto)
        {
            try
            {
                var resultado = await _tipoPagoService.CrearAsync(dto);

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
                    mensaje = "Tipo de pago creado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear tipo de pago",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateTipoPagoDTO dto)
        {
            try
            {
                var resultado = await _tipoPagoService.ActualizarAsync(id, dto);

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
                    mensaje = "Tipo de pago actualizado correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar tipo de pago",
                    detalle = ex.Message
                });
            }
        }
    }
}
