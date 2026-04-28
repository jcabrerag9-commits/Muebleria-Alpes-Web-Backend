using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Asistencia;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/asistencia")]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService _service;

        public AsistenciaController(IAsistenciaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int empleadoId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var resultado = await _service.ListarAsync(empleadoId, fechaInicio, fechaFin);

                return Ok(new
                {
                    mensaje = "Asistencias obtenidas correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener asistencias",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAsistenciaDTO dto)
        {
            try
            {
                var resultado = await _service.RegistrarAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new
                {
                    mensaje = "Asistencia registrada correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al registrar asistencia",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAsistenciaDTO dto)
        {
            try
            {
                var resultado = await _service.ActualizarAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new
                {
                    mensaje = "Asistencia actualizada correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar asistencia",
                    detalle = ex.Message
                });
            }
        }
    }
}
