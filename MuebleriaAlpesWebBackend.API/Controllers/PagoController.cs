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

        private readonly ILogger<PagoController> _logger;

        public PagoController(IPagoService pagoService, ILogger<PagoController> logger)
        {
            _pagoService = pagoService;
            _logger = logger;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarPago([FromBody] ProcesarPagoRequest request, System.Threading.CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("[PAGO REQUEST] {@Request}", request);
                request.UsuarioId ??= ObtenerUsuarioActual();
                var response = await _pagoService.ProcesarPagoAsync(request, ct);

                _logger.LogInformation("[PAGO RESULTADO] {Resultado} {Mensaje}", response.Success, response.Message);
                if (response.Success)
                {
                    return Ok(response);
                }

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "[PAGO ERROR]");
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Error interno del servidor", Data = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pago = await _pagoService.ObtenerPorIdAsync(id);
            if (pago == null) return NotFound(new ApiResponse<PagoDTO> { Success = false, Message = "Pago no encontrado" });
            return Ok(new ApiResponse<PagoDTO> { Success = true, Message = "Pago obtenido", Data = pago });
        }

        [HttpGet("listado")]
        public async Task<IActionResult> GetAll()
        {
            var pagos = await _pagoService.ObtenerTodosAsync();
            _logger.LogInformation("[PAGOS RAW LISTADO] {@Data}", pagos);
            return Ok(new ApiResponse<System.Collections.Generic.IEnumerable<PagoDTO>> { Success = true, Message = "Listado de pagos", Data = pagos });
        }

        [HttpGet("factura/{facturaId}")]
        public async Task<IActionResult> GetByFactura(int facturaId)
        {
            var pagos = await _pagoService.ObtenerPorFacturaIdAsync(facturaId);
            _logger.LogInformation("[PAGOS RAW FACTURA] {@Data}", pagos);
            return Ok(new ApiResponse<System.Collections.Generic.IEnumerable<PagoDTO>> { Success = true, Message = "Pagos por factura", Data = pagos });
        }

        [HttpGet("ordenes-pendientes")]
        public async Task<IActionResult> GetOrdenesPendientes()
        {
            var ordenes = await _pagoService.ObtenerOrdenesPendientesAsync();
            _logger.LogInformation("[ORDENES PENDIENTES API]");
            return Ok(new ApiResponse<System.Collections.Generic.IEnumerable<OrdenPendientePagoDTO>> { Success = true, Message = "Órdenes pendientes de pago", Data = ordenes });
        }

        [HttpPut("anular/{id}")]
        public async Task<IActionResult> Anular(int id, [FromBody] AnularPagoRequest request, System.Threading.CancellationToken ct)
        {
            _logger.LogInformation("[ANULAR PAGO] Id={Id} Motivo={Motivo}", id, request.Motivo);
            request.PagoId = id;
            request.UsuarioId ??= ObtenerUsuarioActual();

            var response = await _pagoService.AnularPagoAsync(id, request.Motivo, request.UsuarioId.Value, ct);
            
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        private int ObtenerUsuarioActual()
        {
            var claimId = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 999;
        }
    }
}
    