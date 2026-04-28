using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{

    [ApiController]
    [Route("api/rh/evaluaciones")]
    public class EvaluacionController : ControllerBase
    {
        private readonly IEvaluacionService _service;

        public EvaluacionController(IEvaluacionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearEvaluacionDTO dto)
        {
            try
            {
                var resultado = await _service.CrearAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new
                {
                    mensaje = "Evaluación creada correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al crear evaluación",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEvaluacionDTO dto)
        {
            try
            {
                var resultado = await _service.ActualizarAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new
                {
                    mensaje = "Evaluación actualizada correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar evaluación",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _service.ObtenerPorIdAsync(id);

                if (resultado == null)
                    return NotFound(new { mensaje = "Evaluación no encontrada" });

                return Ok(new
                {
                    mensaje = "Evaluación obtenida correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener evaluación",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("empleado/{empleadoId:int}")]
        public async Task<IActionResult> ListarPorEmpleado(int empleadoId)
        {
            try
            {
                var resultado = await _service.ListarPorEmpleadoAsync(empleadoId);

                return Ok(new
                {
                    mensaje = "Evaluaciones del empleado obtenidas correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener evaluaciones del empleado",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("reporte")]
        public async Task<IActionResult> Reporte(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var resultado = await _service.ReporteAsync(fechaInicio, fechaFin);

                return Ok(new
                {
                    mensaje = "Reporte de evaluaciones obtenido correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener reporte de evaluaciones",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("empleado/{empleadoId:int}/promedio")]
        public async Task<IActionResult> Promedio(
            int empleadoId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var resultado = await _service.ObtenerPromedioAsync(empleadoId, fechaInicio, fechaFin);

                return Ok(new
                {
                    mensaje = "Promedio de evaluaciones obtenido correctamente",
                    resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener promedio de evaluaciones",
                    detalle = ex.Message
                });
            }
        }
    }
}
