using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/asignaciones")]
    public class AsignacionesEmpleadoController : ControllerBase
    {
        private readonly IAsignacionEmpleadoService _service;

        public AsignacionesEmpleadoController(IAsignacionEmpleadoService service)
        {
            _service = service;
        }

        [HttpPost("departamento")]
        public async Task<IActionResult> AsignarDepartamento([FromBody] AsignarDepartamentoDTO dto)
        {
            try
            {
                var resultado = await _service.AsignarDepartamentoAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Departamento asignado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al asignar departamento", detalle = ex.Message });
            }
        }

        [HttpPatch("departamento/{asignacionId:int}/finalizar")]
        public async Task<IActionResult> FinalizarDepartamento(int asignacionId, [FromBody] FinalizarAsignacionDTO dto)
        {
            try
            {
                var resultado = await _service.FinalizarDepartamentoAsync(asignacionId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Asignación de departamento finalizada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al finalizar asignación de departamento", detalle = ex.Message });
            }
        }

        [HttpGet("empleado/{empleadoId:int}/departamentos")]
        public async Task<IActionResult> ListarDepartamentos(int empleadoId)
        {
            try
            {
                var resultado = await _service.ListarDepartamentosAsync(empleadoId);
                return Ok(new { mensaje = "Asignaciones de departamento obtenidas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener asignaciones de departamento", detalle = ex.Message });
            }
        }

        [HttpPost("puesto")]
        public async Task<IActionResult> AsignarPuesto([FromBody] AsignarPuestoDTO dto)
        {
            try
            {
                var resultado = await _service.AsignarPuestoAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Puesto asignado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al asignar puesto", detalle = ex.Message });
            }
        }

        [HttpPatch("puesto/{asignacionId:int}/finalizar")]
        public async Task<IActionResult> FinalizarPuesto(int asignacionId, [FromBody] FinalizarAsignacionDTO dto)
        {
            try
            {
                var resultado = await _service.FinalizarPuestoAsync(asignacionId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Asignación de puesto finalizada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al finalizar asignación de puesto", detalle = ex.Message });
            }
        }

        [HttpGet("empleado/{empleadoId:int}/puestos")]
        public async Task<IActionResult> ListarPuestos(int empleadoId)
        {
            try
            {
                var resultado = await _service.ListarPuestosAsync(empleadoId);
                return Ok(new { mensaje = "Historial de puestos obtenido correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener historial de puestos", detalle = ex.Message });
            }
        }

        [HttpPost("turno")]
        public async Task<IActionResult> AsignarTurno([FromBody] AsignarTurnoDTO dto)
        {
            try
            {
                var resultado = await _service.AsignarTurnoAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Turno asignado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al asignar turno", detalle = ex.Message });
            }
        }

        [HttpPatch("turno/{asignacionId:int}/finalizar")]
        public async Task<IActionResult> FinalizarTurno(int asignacionId, [FromBody] FinalizarAsignacionDTO dto)
        {
            try
            {
                var resultado = await _service.FinalizarTurnoAsync(asignacionId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Asignación de turno finalizada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al finalizar asignación de turno", detalle = ex.Message });
            }
        }

        [HttpGet("empleado/{empleadoId:int}/turnos")]
        public async Task<IActionResult> ListarTurnos(int empleadoId)
        {
            try
            {
                var resultado = await _service.ListarTurnosAsync(empleadoId);
                return Ok(new { mensaje = "Turnos del empleado obtenidos correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener turnos del empleado", detalle = ex.Message });
            }
        }
    }
}
