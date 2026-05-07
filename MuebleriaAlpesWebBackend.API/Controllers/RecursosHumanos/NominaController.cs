using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.API.Controllers.RecursosHumanos
{
    [ApiController]
    [Route("api/rh/nominas")]
    public class NominaController : ControllerBase
    {
        private readonly INominaService _service;

        public NominaController(INominaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearNominaDTO dto)
        {
            try
            {
                var resultado = await _service.CrearAsync(dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Nómina creada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear nómina", detalle = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] string? estado = null)
        {
            try
            {
                var resultado = await _service.ListarAsync(estado);
                return Ok(new { mensaje = "Nóminas obtenidas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener nóminas", detalle = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _service.ObtenerPorIdAsync(id);

                if (resultado == null)
                    return NotFound(new { mensaje = "Nómina no encontrada" });

                return Ok(new { mensaje = "Nómina obtenida correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener nómina", detalle = ex.Message });
            }
        }

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoNominaDTO dto)
        {
            try
            {
                var resultado = await _service.CambiarEstadoAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Estado de nómina actualizado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cambiar estado de nómina", detalle = ex.Message });
            }
        }

        [HttpPost("{id:int}/empleados")]
        public async Task<IActionResult> AgregarEmpleado(int id, [FromBody] AgregarEmpleadoNominaDTO dto)
        {
            try
            {
                var resultado = await _service.AgregarEmpleadoAsync(id, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Empleado agregado a nómina correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al agregar empleado a nómina", detalle = ex.Message });
            }
        }

        [HttpGet("{id:int}/detalle")]
        public async Task<IActionResult> ListarDetalle(int id)
        {
            try
            {
                var resultado = await _service.ListarDetalleAsync(id);
                return Ok(new { mensaje = "Detalle de nómina obtenido correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener detalle de nómina", detalle = ex.Message });
            }
        }

        [HttpPatch("detalle/{detalleId:int}/calcular")]
        public async Task<IActionResult> CalcularDetalle(int detalleId, [FromBody] CalcularNominaDetalleDTO dto)
        {
            try
            {
                var resultado = await _service.CalcularDetalleAsync(detalleId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Detalle de nómina calculado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular detalle de nómina", detalle = ex.Message });
            }
        }

        [HttpPost("detalle/{detalleId:int}/ingresos")]
        public async Task<IActionResult> AgregarIngreso(int detalleId, [FromBody] AgregarIngresoNominaDTO dto)
        {
            try
            {
                var resultado = await _service.AgregarIngresoAsync(detalleId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Ingreso agregado correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al agregar ingreso", detalle = ex.Message });
            }
        }

        [HttpGet("detalle/{detalleId:int}/ingresos")]
        public async Task<IActionResult> ListarIngresos(int detalleId)
        {
            try
            {
                var resultado = await _service.ListarIngresosAsync(detalleId);
                return Ok(new { mensaje = "Ingresos obtenidos correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener ingresos", detalle = ex.Message });
            }
        }

        [HttpPost("detalle/{detalleId:int}/deducciones")]
        public async Task<IActionResult> AgregarDeduccion(int detalleId, [FromBody] AgregarDeduccionNominaDTO dto)
        {
            try
            {
                var resultado = await _service.AgregarDeduccionAsync(detalleId, dto);

                if (resultado.Resultado == "ERROR")
                    return BadRequest(new { mensaje = resultado.Mensaje, resultado });

                return Ok(new { mensaje = "Deducción agregada correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al agregar deducción", detalle = ex.Message });
            }
        }

        [HttpGet("detalle/{detalleId:int}/deducciones")]
        public async Task<IActionResult> ListarDeducciones(int detalleId)
        {
            try
            {
                var resultado = await _service.ListarDeduccionesAsync(detalleId);
                return Ok(new { mensaje = "Deducciones obtenidas correctamente", resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener deducciones", detalle = ex.Message });
            }
        }
    }
}
