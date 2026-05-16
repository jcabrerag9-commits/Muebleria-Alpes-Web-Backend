using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>Lista todas las categorías activas.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(categorias);
        }

        /// <summary>Obtiene una categoría por Id.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            return categoria is null ? NotFound() : Ok(categoria);
        }

        /// <summary>Lista los productos que pertenecen a una categoría.</summary>
        [HttpGet("{id}/productos")]
        public async Task<IActionResult> GetProductos(int id)
        {
            var productos = await _categoriaService.GetProductosByCategoriaAsync(id);
            return Ok(productos);
        }

        /// <summary>Lista las categorías a las que pertenece un producto.</summary>
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> GetByProducto(int productoId)
        {
            var categorias = await _categoriaService.GetCategoriasByProductoAsync(productoId);
            return Ok(categorias);
        }

        /// <summary>
        /// Crea una nueva categoría. El código se genera automáticamente a partir del nombre
        /// (MAYÚSCULAS, sin espacios, máx. 20 caracteres) — el usuario no lo ingresa.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCategoriaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido." });

            // Generar código desde el nombre: SALA_DE_ESTAR, COMEDOR, etc.
            var codigo = GenerarCodigo(request.Nombre);

            var (ok, mensaje, id) = await _categoriaService.CreateAsync(codigo, request.Nombre, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje, id, codigo });
        }

        /// <summary>
        /// Convierte un nombre a código único: mayúsculas, solo alfanuméricos y guión bajo, máx 20 chars.
        /// Ej: "Sala de Estar" → "SALA_DE_ESTAR"
        /// </summary>
        private static string GenerarCodigo(string nombre)
        {
            var limpio = System.Text.RegularExpressions.Regex.Replace(
                nombre.Trim().ToUpper(), @"[^A-Z0-9]", "_");
            // Colapsar guiones bajos múltiples y quitar los extremos
            limpio = System.Text.RegularExpressions.Regex.Replace(limpio, @"_+", "_").Trim('_');
            return limpio.Length > 20 ? limpio[..20] : limpio;
        }

        /// <summary>Actualiza nombre y descripción de una categoría.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarCategoriaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido." });

            var (ok, mensaje) = await _categoriaService.UpdateAsync(id, request.Nombre, request.Descripcion);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }

        /// <summary>Elimina una categoría (solo si no tiene productos asignados).</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, mensaje) = await _categoriaService.DeleteAsync(id);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }
    }

    // Request DTOs — Codigo se genera automáticamente, el usuario solo ingresa Nombre y Descripcion
    public record CrearCategoriaRequest(string Nombre, string? Descripcion);
    public record ActualizarCategoriaRequest(string Nombre, string? Descripcion);
}
