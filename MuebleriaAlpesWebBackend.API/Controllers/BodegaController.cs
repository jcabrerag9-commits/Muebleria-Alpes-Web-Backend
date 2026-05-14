using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly IBodegaService _bodegaService;

        public BodegaController(IBodegaService bodegaService)
        {
            _bodegaService = bodegaService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool soloActivas = false)
        {
            var bodegas = await _bodegaService.ListarBodegasAsync(soloActivas);
            return Ok(new InventarioResponse<IEnumerable<BodegaDTO>> { Resultado = "EXITO", Data = bodegas });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var bodega = await _bodegaService.ObtenerBodegaPorIdAsync(id);
            if (bodega == null) 
            {
                return NotFound(new InventarioResponse<BodegaDTO> 
                { 
                    Success = false, 
                    Message = "Bodega no encontrada" 
                });
            }
            return Ok(new InventarioResponse<BodegaDTO> { Resultado = "EXITO", Data = bodega });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] BodegaDTO bodega, [FromQuery] int usuarioId = 1)
        {
            if (bodega == null) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Datos de bodega no recibidos o JSON inválido." });
            if (!ModelState.IsValid) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Payload inválido", Errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            
            Console.WriteLine($"[API BODEGA] Recibido POST: Nombre={bodega.Nombre}, Tipo={bodega.Tipo}, Canal={bodega.CanalVentaId}");
            var result = await _bodegaService.CrearBodegaAsync(bodega, usuarioId);
            if (!result.IsSuccess) 
            {
                Console.WriteLine($"[API BODEGA ERROR] {result.Mensaje}");
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] BodegaDTO bodega, [FromQuery] int usuarioId = 1)
        {
            if (bodega == null) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Datos de bodega no recibidos o JSON inválido." });
            if (!ModelState.IsValid) return BadRequest(new InventarioResponse<object> { Success = false, Resultado = "ERROR", Message = "Payload inválido", Errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            
            Console.WriteLine($"[API BODEGA] Recibido PUT para ID {id}: Nombre={bodega.Nombre}");
            bodega.BodegaId = id;
            var result = await _bodegaService.ActualizarBodegaAsync(bodega, usuarioId);
            if (!result.IsSuccess) 
            {
                Console.WriteLine($"[API BODEGA ERROR] {result.Mensaje}");
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] string estado, [FromQuery] string motivo, [FromQuery] int usuarioId = 1)
        {
            var result = await _bodegaService.CambiarEstadoBodegaAsync(id, estado, motivo, usuarioId);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
