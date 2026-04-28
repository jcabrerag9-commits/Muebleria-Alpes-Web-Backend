using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Carrito;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarProducto([FromBody] AgregarProductoCarritoRequestDto request)
        {
            try
            {
                var resultado = await _carritoService.AgregarProductoAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al agregar producto", detalle = ex.Message });
            }
        }

        [HttpPut("actualizar-cantidad")]
        public async Task<IActionResult> ActualizarCantidad([FromBody] ActualizarCantidadCarritoRequestDto request)
        {
            try
            {
                var resultado = await _carritoService.ActualizarCantidadAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al actualizar cantidad", detalle = ex.Message });
            }
        }

        [HttpDelete("eliminar/{detalleId}")]
        public async Task<IActionResult> EliminarProducto(int detalleId)
        {
            try
            {
                var resultado = await _carritoService.EliminarProductoAsync(detalleId);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al eliminar producto", detalle = ex.Message });
            }
        }

        [HttpDelete("vaciar/{carritoId}")]
        public async Task<IActionResult> VaciarCarrito(int carritoId)
        {
            try
            {
                var resultado = await _carritoService.VaciarCarritoAsync(carritoId);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al vaciar carrito", detalle = ex.Message });
            }
        }

        [HttpGet("total/{carritoId}")]
        public async Task<IActionResult> CalcularTotal(int carritoId)
        {
            try
            {
                var resultado = await _carritoService.CalcularTotalAsync(carritoId);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al calcular total", detalle = ex.Message });
            }
        }

        [HttpPost("convertir-orden")]
        public async Task<IActionResult> ConvertirOrden([FromBody] ConvertirOrdenCarritoRequestDto request)
        {
            try
            {
                var resultado = await _carritoService.ConvertirOrdenAsync(request);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al convertir a orden", detalle = ex.Message });
            }
        }
    }
}
