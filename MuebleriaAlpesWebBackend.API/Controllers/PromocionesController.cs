using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/promociones")]
    public class PromocionesController : ControllerBase
    {
        private readonly IPromocionService _service;

        public PromocionesController(IPromocionService service)
        {
            _service = service;
        }

        // GET /api/promociones?estado=ACTIVO&tipo=PORCENTAJE
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? estado, [FromQuery] string? tipo)
        {
            var resultado = await _service.GetAllAsync(estado, tipo);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/promociones/vigentes
        [HttpGet("vigentes")]
        public async Task<IActionResult> GetVigentes()
        {
            var resultado = await _service.GetVigentesAsync();
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/promociones/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Promoción ID {id} no encontrada." });

            return Ok(new { success = true, data = resultado });
        }

        // GET /api/promociones/codigo/{codigo}
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var resultado = await _service.GetByCodigoAsync(codigo);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Promoción con código '{codigo}' no encontrada." });

            return Ok(new { success = true, data = resultado });
        }

        // POST /api/promociones
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromocionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Datos inválidos.", errors = errores });
            }

            try
            {
                var creada = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = creada.PrmPromocion },
                    new { success = true, message = "Promoción creada exitosamente.", data = creada });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT /api/promociones/{id}
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] PromocionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Datos inválidos.", errors = errores });
            }

            try
            {
                var actualizada = await _service.UpdateAsync(id, dto);
                if (actualizada is null)
                    return NotFound(new { success = false, message = $"Promoción ID {id} no encontrada." });

                return Ok(new { success = true, message = "Promoción actualizada.", data = actualizada });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // DELETE /api/promociones/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { success = false, message = $"Promoción ID {id} no encontrada." });

            return Ok(new { success = true, message = "Promoción eliminada exitosamente." });
        }

        // POST /api/promociones/{id}/productos
        [HttpPost("{id:long}/productos")]
        public async Task<IActionResult> AddProducto(long id, [FromBody] PromocionProductoCreateDto dto)
        {
            try
            {
                var resultado = await _service.AddProductoAsync(id, dto);
                return StatusCode(201, new { success = true, message = "Producto agregado.", data = resultado });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // DELETE /api/promociones/{id}/productos/{ppoId}
        [HttpDelete("{id:long}/productos/{ppoId:long}")]
        public async Task<IActionResult> RemoveProducto(long id, long ppoId)
        {
            try
            {
                var eliminado = await _service.RemoveProductoAsync(id, ppoId);
                if (!eliminado)
                    return NotFound(new { success = false, message = $"Relación producto-promoción ID {ppoId} no encontrada." });

                return Ok(new { success = true, message = "Producto eliminado de la promoción." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}
