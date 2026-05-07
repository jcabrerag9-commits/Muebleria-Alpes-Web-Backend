using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContenidoController : ControllerBase
    {
        private readonly IContenidoService _contenidoService;

        public ContenidoController(IContenidoService contenidoService)
        {
            _contenidoService = contenidoService;
        }

        [HttpGet("producto/{productoId}/imagenes")]
        public async Task<IActionResult> GetImagenes(int productoId)
        {
            var result = await _contenidoService.GetImagenesByProductoIdAsync(productoId);
            return Ok(result);
        }

        [HttpPost("imagenes")]
        public async Task<IActionResult> CreateImagen([FromBody] ProductoImagen imagen)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _contenidoService.CreateImagenAsync(imagen);
            return Ok(new { id });
        }

        [HttpPut("imagenes/{id}")]
        public async Task<IActionResult> UpdateImagen(int id, [FromBody] ProductoImagen imagen)
        {
            imagen.Id = id;
            await _contenidoService.UpdateImagenAsync(imagen);
            return NoContent();
        }

        [HttpPatch("producto/{productoId}/imagenes/{imagenId}/principal")]
        public async Task<IActionResult> SetPrincipal(int productoId, int imagenId)
        {
            await _contenidoService.SetImagenPrincipalAsync(productoId, imagenId);
            return NoContent();
        }

        [HttpDelete("imagenes/{id}")]
        public async Task<IActionResult> DeleteImagen(int id)
        {
            await _contenidoService.DeleteImagenAsync(id);
            return NoContent();
        }

        [HttpPost("traducciones")]
        public async Task<IActionResult> UpsertTraduccion([FromBody] ProductoTraduccion traduccion)
        {
            await _contenidoService.UpsertTraduccionAsync(traduccion);
            return Ok();
        }
    }
}
