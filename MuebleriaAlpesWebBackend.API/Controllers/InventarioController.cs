using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MuebleriaAlpesWebBackend.API.Controllers
{
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

        [HttpGet("reservas/{productoId}")]
        public async Task<IActionResult> GetReservas(int productoId)
        {
            var reservas = await _inventarioService.ObtenerReservasPorProductoAsync(productoId);
            return Ok(new InventarioResponse<IEnumerable<ReservaDTO>>
            {
                Resultado = "EXITO",
                Data = reservas
            });
        }


        [HttpGet("kardex/{productoId}")]
        public async Task<IActionResult> GetKardex(int productoId)
        {
            var kardex = await _inventarioService.ObtenerKardexPorProductoAsync(productoId);
            return Ok(new InventarioResponse<IEnumerable<KardexDTO>>
            {
                Resultado = "EXITO",
                Data = kardex
            });
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovimientosGlobales([FromQuery] MovimientoFiltroRequest filtro)
        {
            var movimientos = await _inventarioService.ObtenerMovimientosGlobalesAsync(filtro);
            return Ok(new InventarioResponse<IEnumerable<KardexDTO>>
            {
                Resultado = "EXITO",
                Data = movimientos
            });
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimientoInventarioRequest request)
        {
            if (request == null) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Datos de entrada no recibidos o JSON inválido." });
            if (!ModelState.IsValid) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Payload inválido", Errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            
            try
            {
                Console.WriteLine($"[API] Entrada recibida: Prod={request.ProductoId}, Bod={request.BodegaId}, Cant={request.Cantidad}");
                var response = await _inventarioService.RegistrarEntradaAsync(request);
                if (!response.IsSuccess) return BadRequest(response);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[API EXCEPTION] RegistrarEntrada: {ex.Message}");
                return StatusCode(500, new InventarioResponse<object> 
                { 
                    Success = false, 
                    Message = ex.Message,
                    Errores = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("salida")]
        public async Task<IActionResult> RegistrarSalida([FromBody] MovimientoInventarioRequest request)
        {
            if (request == null) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Datos de salida no recibidos o JSON inválido." });
            if (!ModelState.IsValid) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Payload inválido", Errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            try
            {
                Console.WriteLine($"[API] Salida recibida: Prod={request.ProductoId}, Bod={request.BodegaId}, Cant={request.Cantidad}");
                var response = await _inventarioService.RegistrarSalidaAsync(request);
                if (!response.IsSuccess) return BadRequest(response);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[API EXCEPTION] RegistrarSalida: {ex.Message}");
                return StatusCode(500, new InventarioResponse<object> 
                { 
                    Success = false, 
                    Message = ex.Message,
                    Errores = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> ReservarStock([FromBody] ReservaStockRequest request)
        {
            if (request == null) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Datos de reserva no recibidos o JSON inválido." });
            if (!ModelState.IsValid) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Payload inválido", Errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

            try
            {
                Console.WriteLine($"[API] Reserva recibida: Prod={request.ProductoId}, Bod={request.BodegaId}, Cant={request.Cantidad}");
                var response = await _inventarioService.ReservarStockAsync(request);
                if (!response.IsSuccess) return BadRequest(response);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[API EXCEPTION] ReservarStock: {ex.Message}");
                return StatusCode(500, new InventarioResponse<object> 
                { 
                    Success = false, 
                    Message = ex.Message,
                    Errores = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("liberar/{reservaId}")]
        public async Task<IActionResult> Liberar(int reservaId)
        {
            // Usa el método con validación de regla de negocio ERP.
            // RECHAZADO = HTTP 403 (el cliente sabe que la operación está prohibida, no que falló).
            var response = await _inventarioService.ValidarYLiberarReservaAsync(reservaId);
            
            if (response.Resultado == "RECHAZADO")
                return StatusCode(403, response);
            
            if (!response.IsSuccess)
                return BadRequest(response);
            
            return Ok(response);
        }
    }
}
