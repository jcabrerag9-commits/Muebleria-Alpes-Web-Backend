using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/banners")]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _service;

        public BannersController(IBannerService service)
        {
            _service = service;
        }

        // GET /api/banners?estado=ACTIVO&posicion=HOME_PRINCIPAL
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? estado, [FromQuery] string? posicion)
        {
            var resultado = await _service.GetAllAsync(estado, posicion);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/banners/vigentes?posicion=HOME_PRINCIPAL
        [HttpGet("vigentes")]
        public async Task<IActionResult> GetVigentes([FromQuery] string? posicion)
        {
            var resultado = await _service.GetVigentesAsync(posicion);
            return Ok(new { success = true, data = resultado });
        }

        // GET /api/banners/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Banner ID {id} no encontrado." });

            return Ok(new { success = true, data = resultado });
        }

        // GET /api/banners/{id}/estadisticas
        [HttpGet("{id:long}/estadisticas")]
        public async Task<IActionResult> GetEstadisticas(long id)
        {
            var resultado = await _service.GetEstadisticasAsync(id);
            if (resultado is null)
                return NotFound(new { success = false, message = $"Banner ID {id} no encontrado." });

            return Ok(new { success = true, data = resultado });
        }

        // POST /api/banners
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BannerCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Datos inválidos.", errors = errores });
            }

            try
            {
                var creado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = creado.BanBanner },
                    new { success = true, message = "Banner creado exitosamente.", data = creado });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT /api/banners/{id}
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] BannerUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Datos inválidos.", errors = errores });
            }

            try
            {
                var actualizado = await _service.UpdateAsync(id, dto);
                if (actualizado is null)
                    return NotFound(new { success = false, message = $"Banner ID {id} no encontrado." });

                return Ok(new { success = true, message = "Banner actualizado.", data = actualizado });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // DELETE /api/banners/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { success = false, message = $"Banner ID {id} no encontrado." });

            return Ok(new { success = true, message = "Banner eliminado exitosamente." });
        }

        // POST /api/banners/{id}/clicks
        [HttpPost("{id:long}/clicks")]
        public async Task<IActionResult> RegistrarClick(long id, [FromBody] ClickBannerCreateDto dto)
        {
            try
            {
                var resultado = await _service.RegistrarClickAsync(id, dto);
                return StatusCode(201, new { success = true, message = "Click registrado.", data = resultado });
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
    }
}