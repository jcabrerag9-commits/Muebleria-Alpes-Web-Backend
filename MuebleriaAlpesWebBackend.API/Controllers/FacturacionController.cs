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

        public FacturacionController(IFacturacionService facturacionService)
        {
            _facturacionService = facturacionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturacionService.ObtenerFacturaPorIdAsync(id);
            if (factura == null)
            {
                return NotFound(new FacturacionResponse<FacturaDTO>
                {
                    Resultado = "ERROR",
                    Mensaje = "Factura no encontrada.",
                    Data = null
                });
            }

            return Ok(new FacturacionResponse<FacturaDTO>
            {
                Resultado = "EXITO",
                Mensaje = "Consulta exitosa.",
                Data = factura
            });
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var facturas = await _facturacionService.ObtenerFacturasPorClienteAsync(clienteId);
            return Ok(new FacturacionResponse<IEnumerable<FacturaDTO>>
            {
                Resultado = "EXITO",
                Mensaje = "Consulta exitosa.",
                Data = facturas
            });
        }

        [HttpPost("generar")]
        public async Task<IActionResult> Generar([FromBody] GenerarFacturaRequest request)
        {
            var response = await _facturacionService.GenerarFacturaAsync(request);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("anular/{id}")]
        public async Task<IActionResult> Anular(int id, [FromBody] AnularFacturaRequest request)
        {
            // El ID del body debe coincidir con el de la URL por seguridad
            request.FacturaId = id;
            var response = await _facturacionService.AnularFacturaAsync(request);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
