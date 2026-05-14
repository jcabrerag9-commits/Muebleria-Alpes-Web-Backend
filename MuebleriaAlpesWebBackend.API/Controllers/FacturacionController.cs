using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturacionController : ControllerBase
    {
        private readonly IFacturacionService _facturacionService;
        private readonly Microsoft.Extensions.Logging.ILogger<FacturacionController> _logger;

        public FacturacionController(IFacturacionService facturacionService, Microsoft.Extensions.Logging.ILogger<FacturacionController> logger)
        {
            _facturacionService = facturacionService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturacionService.ObtenerFacturaPorIdAsync(id);
            if (factura == null)
            {
                return NotFound(new ApiResponse<FacturaDTO>
                {
                    Success = false,
                    Message = "Factura no encontrada.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<FacturaDTO>
            {
                Success = true,
                Message = "Consulta exitosa.",
                Data = factura
            });
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var facturas = await _facturacionService.ObtenerFacturasPorClienteAsync(clienteId);
            return Ok(new ApiResponse<IEnumerable<FacturaDTO>>
            {
                Success = true,
                Message = "Consulta exitosa.",
                Data = facturas
            });
        }

        [HttpPost("generar")]
        public async Task<IActionResult> Generar([FromBody] GenerarFacturaRequest request)
        {
            _logger.LogInformation("[FACTURA REQUEST] {@Request}", request);
            var response = await _facturacionService.GenerarFacturaAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("anular/{id}")]
        public async Task<IActionResult> Anular(int id, [FromBody] AnularFacturaRequest request)
        {
            _logger.LogInformation("[MOTIVO ANULACION] {Motivo}", request.Motivo);
            request.FacturaId = id;
            var response = await _facturacionService.AnularFacturaAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("listado")]
        public async Task<IActionResult> GetAll([FromQuery] string? estado = null, [FromQuery] int? clienteId = null, [FromQuery] string? nit = null)
        {
            var facturas = await _facturacionService.ObtenerTodasAsync(estado, clienteId, nit);
            _logger.LogInformation("FACTURAS RAW {@Data}", facturas);
            return Ok(new ApiResponse<IEnumerable<FacturaDTO>>
            {
                Success = true,
                Message = "Consulta general exitosa.",
                Data = facturas
            });
        }

        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> GetDetalle(int id)
        {
            var detalle = await _facturacionService.ObtenerDetallePorIdAsync(id);
            if (detalle == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Detalle de factura no encontrado.",
                    Data = null
                });
            }

            _logger.LogInformation("[FACTURA RAW API] {@Factura}", detalle);
            return Ok(new ApiResponse<FacturaDTO>
            {
                Success = true,
                Message = "Detalle obtenido con éxito.",
                Data = detalle
            });
        }

        [HttpGet("ordenes-pendientes")]
        public async Task<IActionResult> GetOrdenesPendientes()
        {
            var ordenes = await _facturacionService.ObtenerOrdenesPendientesAsync();
            _logger.LogInformation("[FACTURA ORDENES PENDIENTES] Count={Count}", ordenes.Count());
            return Ok(new ApiResponse<IEnumerable<OrdenPendienteDTO>>
            {
                Success = true,
                Message = "Órdenes pendientes obtenidas con éxito.",
                Data = ordenes
            });
        }
    }
}
