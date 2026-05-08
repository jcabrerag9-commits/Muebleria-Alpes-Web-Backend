using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(new InventarioResponse<IEnumerable<ProductoDTO>>
            {
                Resultado = "EXITO",
                Data = productos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(new InventarioResponse<ProductoDTO>
            {
                Resultado = "EXITO",
                Data = producto
            });
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearProductoRequest request)
        {
            var response = await _productoService.CrearProductoAsync(request);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("variante")]
        public async Task<IActionResult> CrearVariante([FromBody] CrearVarianteRequest request)
        {
            var response = await _productoService.CrearVarianteAsync(request);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet("existencia/{productoId}")]
        public async Task<IActionResult> GetExistencia(int productoId)
        {
            var existencias = await _inventarioService.ObtenerExistenciasPorProductoAsync(productoId);
            return Ok(new InventarioResponse<IEnumerable<ExistenciaDTO>>
            {
                Resultado = "EXITO",
                Data = existencias
            });
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimientoInventarioRequest request)
        {
            var response = await _inventarioService.RegistrarEntradaAsync(request);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("salida")]
        public async Task<IActionResult> RegistrarSalida([FromBody] MovimientoInventarioRequest request)
        {
            var response = await _inventarioService.RegistrarSalidaAsync(request);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] ReservaStockRequest request)
        {
            var response = await _inventarioService.ReservarStockAsync(request);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("liberar/{reservaId}")]
        public async Task<IActionResult> Liberar(int reservaId)
        {
            var response = await _inventarioService.LiberarReservaAsync(reservaId);
            if (!response.IsSuccess) return BadRequest(response);
            return Ok(response);
        }
    }
}
