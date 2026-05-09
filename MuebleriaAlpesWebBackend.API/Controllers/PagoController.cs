using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost("procesar")]
        [ProducesResponseType(typeof(ProcesarPagoResponse), 200)]
        [ProducesResponseType(typeof(ProcesarPagoResponse), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ProcesarPago([FromBody] ProcesarPagoRequest request)
        {
            try
            {
                var response = await _pagoService.ProcesarPagoAsync(request);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                // Error de negocio devuelto por el SP
                return BadRequest(response);
            }
            catch (System.Exception ex)
            {
                // Error de sistema (conexión, base de datos caída, etc.)
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        // --- NUEVOS ENDPOINTS ---

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pago = await _pagoService.ObtenerPorIdAsync(id);
            if (pago == null) return NotFound(new { mensaje = "Pago no encontrado" });
            return Ok(pago);
        }

        [HttpGet("listado")]
        public async Task<IActionResult> GetAll()
        {
            var pagos = await _pagoService.ObtenerTodosAsync();
            return Ok(pagos);
        }

        [HttpGet("factura/{facturaId}")]
        public async Task<IActionResult> GetByFactura(int facturaId)
        {
            var pagos = await _pagoService.ObtenerPorFacturaIdAsync(facturaId);
            return Ok(pagos);
        }

        [HttpPut("anular/{id}")]
        public async Task<IActionResult> Anular(int id)
        {
            var resultado = await _pagoService.AnularPagoAsync(id);
            if (!resultado) return BadRequest(new { mensaje = "No se pudo anular el pago" });
            return Ok(new { mensaje = "Pago anulado correctamente" });
        }
    }
}
