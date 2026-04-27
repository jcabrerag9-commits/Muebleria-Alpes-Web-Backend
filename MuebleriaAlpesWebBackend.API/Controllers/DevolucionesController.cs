using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Devoluciones;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/devoluciones")]
    public class DevolucionesController : ControllerBase
    {
        private readonly IDevolucionService _service;

        public DevolucionesController(IDevolucionService service)
        {
            _service = service;
        }

        // GET /api/devoluciones/categorias
        [HttpGet("categorias")]
        public async Task<IActionResult> GetCategorias([FromQuery] string? estado)
        {
            var resultado = await _service.GetCategoriasAsync(estado);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones/categorias/{id}
        [HttpGet("categorias/{id:long}")]
        public async Task<IActionResult> GetCategoriaById(long id)
        {
            var resultado = await _service.GetCategoriaByIdAsync(id);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Categoría ID {id} no encontrada." });

            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones?estado=SOLICITADA&clienteId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? estado, [FromQuery] long? clienteId)
        {
            var resultado = await _service.GetAllAsync(estado, clienteId);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Devolución ID {id} no encontrada." });

            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones/rma/{numeroRma}
        [HttpGet("rma/{numeroRma}")]
        public async Task<IActionResult> GetByRma(string numeroRma)
        {
            var resultado = await _service.GetByRmaAsync(numeroRma);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Devolución RMA '{numeroRma}' no encontrada." });

            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones/orden/{ordenVentaId}
        [HttpGet("orden/{ordenVentaId:long}")]
        public async Task<IActionResult> GetByOrdenVenta(long ordenVentaId)
        {
            var resultado = await _service.GetByOrdenVentaAsync(ordenVentaId);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/devoluciones/cliente/{clienteId}
        [HttpGet("cliente/{clienteId:long}")]
        public async Task<IActionResult> GetByCliente(long clienteId)
        {
            var resultado = await _service.GetByClienteAsync(clienteId);
            return Ok(new { success = true, data = resultado });
        }

        // POST /api/devoluciones
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DevolucionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Datos inválidos.", errors = errores });
            }

            try
            {
                var creada = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = creada.DevDevolucion },
                    new { success = true, message = $"Devolución registrada. RMA: {creada.DevNumeroRma}", data = creada });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
        }

        // PATCH /api/devoluciones/{id}/estado
        [HttpPatch("{id:long}/estado")]
        public async Task<IActionResult> CambiarEstado(long id, [FromBody] DevolucionUpdateEstadoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Estado inválido." });

            try
            {
                var resultado = await _service.CambiarEstadoAsync(id, dto);
                if (resultado is null)
                    return NotFound(new { success = false, message = $"Devolución ID {id} no encontrada." });

                return Ok(new { success = true, message = $"Estado actualizado a '{resultado.DevEstado}'.", data = resultado });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
        }

        // DELETE /api/devoluciones/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var eliminado = await _service.DeleteAsync(id);
                if (!eliminado)
                    return NotFound(new { success = false, message = $"Devolución ID {id} no encontrada." });

                return Ok(new { success = true, message = "Devolución eliminada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
        }
    }
}
