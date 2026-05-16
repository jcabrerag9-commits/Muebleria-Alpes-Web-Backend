using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialesController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialesController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materiales = await _materialService.GetAllAsync();
            return Ok(materiales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            return material is null ? NotFound() : Ok(material);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearMaterialRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre del material es requerido." });

            var (ok, mensaje, id) = await _materialService.CreateAsync(request.Nombre, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje, id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarMaterialRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre del material es requerido." });

            var (ok, mensaje) = await _materialService.UpdateAsync(id, request.Nombre, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, mensaje) = await _materialService.DeleteAsync(id);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }
    }

    public record CrearMaterialRequest(string Nombre, string? Descripcion);
    public record ActualizarMaterialRequest(string Nombre, string? Descripcion);
}
