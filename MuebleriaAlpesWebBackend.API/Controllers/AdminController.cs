using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Admin;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("ordenes")]
        public async Task<IActionResult> ListarOrdenes([FromQuery] ListarOrdenesFiltroDto filtro)
        {
            try
            {
                var resultado = await _adminService.ListarOrdenesAsync(filtro);
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al listar las órdenes", detalle = ex.Message });
            }
        }

        [HttpGet("pagos")]
        public async Task<IActionResult> ListarPagos()
        {
            try
            {
                var resultado = await _adminService.ListarPagosAsync();
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al listar los pagos", detalle = ex.Message });
            }
        }

        [HttpGet("facturas")]
        public async Task<IActionResult> ListarFacturas()
        {
            try
            {
                var resultado = await _adminService.ListarFacturasAsync();
                return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resultado = "ERROR", mensaje = "Error interno al listar las facturas", detalle = ex.Message });
            }
        }
    }
}
