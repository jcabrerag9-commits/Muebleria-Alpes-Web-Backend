using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("[API] GET /api/Productos solicitado");
            var result = await _productoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("[API] GET /api/Productos/{Id} solicitado", id);
            var result = await _productoService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Producto producto)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            _logger.LogInformation("[AUDITORÍA-API] POST /api/Productos solicitado a las {Time} para: {Nombre}", timestamp, producto?.Nombre);
            _logger.LogInformation("[AUDITORÍA-API] Payload Precios -> Base: {P1}, Oferta: {P2}", producto?.PrecioVigente, producto?.PrecioOferta);
            
            if (producto == null) return BadRequest("El producto no puede ser nulo");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[AUDITORÍA-API] ModelState inválido para creación de producto");
                return BadRequest(ModelState);
            }

            try
            {
                await _productoService.CreateAsync(producto);
                _logger.LogInformation("[AUDITORÍA-API] Producto creado exitosamente con ID: {Id}", producto.Id);
                return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR-API] Fallo al crear producto: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Producto producto)
        {
            _logger.LogInformation("[AUDITORÍA-API] PUT /api/Productos/{Id} recibido.", id);
            
            if (producto == null)
            {
                _logger.LogWarning("[AUDITORÍA-API] El objeto producto recibido es NULO para ID: {Id}", id);
                return BadRequest(new { error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            _logger.LogInformation("[AUDITORÍA-API] Datos vinculados (Binding): Nombre='{Nombre}', TipoMueble={TM}, Peso={Peso}, Estado='{Estado}'", 
                producto.Nombre, producto.TipoMueble, producto.Peso, producto.Estado);
            _logger.LogInformation("[AUDITORÍA-API] Datos Precios (Binding): Vigente={P1}, Oferta={P2}", producto.PrecioVigente, producto.PrecioOferta);
            
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("[AUDITORÍA-API] ModelState INVÁLIDO para ID {Id}: {Errors}", id, errors);
                return BadRequest(new { error = "Validación fallida", details = errors });
            }

            try 
            {
                producto.Id = id;
                _logger.LogInformation("[AUDITORÍA-API] Llamando al servicio para actualizar ID: {Id}", id);
                await _productoService.UpdateAsync(producto);
                _logger.LogInformation("[AUDITORÍA-API] Proceso completado con éxito para ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR-API] Excepción al actualizar producto {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { error = "Error interno al procesar la actualización", detail = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string estado)
        {
            await _productoService.ChangeStatusAsync(id, estado);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productoService.DeleteLogicoAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/dimensiones")]
        public async Task<IActionResult> UpsertDimension(int id, [FromBody] DimensionProducto dimension)
        {
            dimension.ProductoId = id;
            await _productoService.UpsertDimensionAsync(dimension);
            return Ok();
        }

        [HttpPost("{id}/categorias/{categoriaId}")]
        public async Task<IActionResult> AsignarCategoria(int id, int categoriaId)
        {
            await _productoService.AsignarCategoriaAsync(id, categoriaId);
            return Ok();
        }

        [HttpDelete("{id}/categorias/{categoriaId}")]
        public async Task<IActionResult> QuitarCategoria(int id, int categoriaId)
        {
            await _productoService.QuitarCategoriaAsync(id, categoriaId);
            return Ok();
        }

        [HttpPost("{id}/colecciones/{coleccionId}")]
        public async Task<IActionResult> AsignarColeccion(int id, int coleccionId)
        {
            await _productoService.AsignarColeccionAsync(id, coleccionId);
            return Ok();
        }

        [HttpDelete("{id}/colecciones/{coleccionId}")]
        public async Task<IActionResult> QuitarColeccion(int id, int coleccionId)
        {
            await _productoService.QuitarColeccionAsync(id, coleccionId);
            return Ok();
        }

        [HttpPost("{id}/materiales/{materialId}")]
        public async Task<IActionResult> AsignarMaterial(int id, int materialId)
        {
            await _productoService.AsignarMaterialAsync(id, materialId);
            return Ok();
        }

        [HttpDelete("{id}/materiales/{materialId}")]
        public async Task<IActionResult> QuitarMaterial(int id, int materialId)
        {
            await _productoService.QuitarMaterialAsync(id, materialId);
            return Ok();
        }

        [HttpPost("{id}/colores/{colorId}")]
        public async Task<IActionResult> AsignarColor(int id, int colorId)
        {
            await _productoService.AsignarColorAsync(id, colorId);
            return Ok();
        }

        [HttpDelete("{id}/colores/{colorId}")]
        public async Task<IActionResult> QuitarColor(int id, int colorId)
        {
            await _productoService.QuitarColorAsync(id, colorId);
            return Ok();
        }

        [HttpPost("resenas")]
        public async Task<IActionResult> CreateResena([FromBody] ResenaProducto resena)
        {
            var id = await _productoService.CreateResenaAsync(resena);
            return Ok(new { id });
        }

        [HttpPatch("resenas/{id}/aprobar")]
        public async Task<IActionResult> AprobarResena(int id)
        {
            await _productoService.AprobarResenaAsync(id);
            return NoContent();
        }
    }
}
