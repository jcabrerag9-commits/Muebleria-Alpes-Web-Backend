using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColoresController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColoresController(IColorService colorService)
        {
            _colorService = colorService;
        }

        /// <summary>Lista todos los colores (activos e inactivos).</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colores = await _colorService.GetAllAsync();
            return Ok(colores);
        }

        /// <summary>Obtiene un color por Id.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _colorService.GetByIdAsync(id);
            return color is null ? NotFound() : Ok(color);
        }

        /// <summary>
        /// Crea un nuevo color. El código se genera automáticamente desde el nombre.
        /// El hex debe ser un color válido (#RRGGBB) — el input type="color" lo garantiza.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearColorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre del color es requerido." });

            var hex = string.IsNullOrWhiteSpace(request.HexColor) ? "#000000" : request.HexColor;

            var (ok, mensaje, id) = await _colorService.CreateAsync(request.Nombre, hex, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje, id });
        }

        /// <summary>Actualiza nombre, hex y descripción de un color.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarColorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre del color es requerido." });

            var hex = string.IsNullOrWhiteSpace(request.HexColor) ? "#000000" : request.HexColor;

            var (ok, mensaje) = await _colorService.UpdateAsync(id, request.Nombre, hex, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }

        /// <summary>Elimina (inactiva) un color si no tiene productos asignados.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, mensaje) = await _colorService.DeleteAsync(id);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }
    }

    // Request DTOs
    public record CrearColorRequest(string Nombre, string? HexColor, string? Descripcion);
    public record ActualizarColorRequest(string Nombre, string? HexColor, string? Descripcion);
}
