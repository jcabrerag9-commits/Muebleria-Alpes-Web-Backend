using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariantesController : ControllerBase
    {
        private readonly IVarianteService _varianteService;

        public VariantesController(IVarianteService varianteService)
        {
            _varianteService = varianteService;
        }

        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> GetByProducto(int productoId)
        {
            var result = await _varianteService.GetByProductoIdAsync(productoId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoVariante variante)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _varianteService.CreateAsync(variante);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoVariante variante)
        {
            variante.Id = id;
            await _varianteService.UpdateAsync(variante);
            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string estado)
        {
            await _varianteService.ChangeStatusAsync(id, estado);
            return NoContent();
        }

        [HttpPost("{id}/atributos/{atributoValorId}")]
        public async Task<IActionResult> AsignarAtributo(int id, int atributoValorId)
        {
            await _varianteService.AsignarAtributoAsync(id, atributoValorId);
            return Ok();
        }

        [HttpDelete("{id}/atributos/{atributoValorId}")]
        public async Task<IActionResult> QuitarAtributo(int id, int atributoValorId)
        {
            await _varianteService.QuitarAtributoAsync(id, atributoValorId);
            return Ok();
        }
    }
}
