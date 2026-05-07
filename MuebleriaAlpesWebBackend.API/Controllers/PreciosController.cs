using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreciosController : ControllerBase
    {
        private readonly IPrecioService _precioService;

        public PreciosController(IPrecioService precioService)
        {
            _precioService = precioService;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] PrecioProducto precio)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _precioService.RegistrarPrecioAsync(precio);
            return Ok(new { id, mensaje = "Precio registrado exitosamente" });
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarPrecioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _precioService.ActualizarPrecioAsync(request);
            return NoContent();
        }

        [HttpGet("vigente/{productoId}")]
        public async Task<IActionResult> GetVigente(int productoId, [FromQuery] int monedaId = 1)
        {
            var precio = await _precioService.GetPrecioVigenteAsync(productoId, monedaId);
            return Ok(new { productoId, monedaId, precio });
        }

        [HttpGet("final/{productoId}")]
        public async Task<IActionResult> GetPrecioFinal(int productoId, [FromQuery] int monedaId = 1)
        {
            var precioFinal = await _precioService.GetPrecioFinalAsync(productoId, monedaId);
            return Ok(new { productoId, monedaId, precioFinal });
        }

        [HttpGet("historial/{productoId}")]
        public async Task<IActionResult> GetHistorial(int productoId)
        {
            var historial = await _precioService.GetHistorialAsync(productoId);
            return Ok(historial);
        }
    }
}
