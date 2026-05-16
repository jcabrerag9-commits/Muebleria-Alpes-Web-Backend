using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanzasController : ControllerBase
    {
        private readonly IFinanzasService _finanzasService;

        public FinanzasController(IFinanzasService finanzasService)
        {
            _finanzasService = finanzasService;
        }

        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial([FromQuery] HistorialFiltroRequest request, System.Threading.CancellationToken ct)
        {
            try
            {
                request.UsuarioId ??= ObtenerUsuarioActual();
                var historial = await _finanzasService.ObtenerHistorialFinancieroAsync(request, ct);
                return Ok(new ApiResponse<System.Collections.Generic.IEnumerable<HistorialFinancieroDTO>>
                {
                    Success = true,
                    Message = "Historial obtenido",
                    Data = historial
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error al obtener el historial financiero",
                    Data = ex.Message
                });
            }
        }

        private int ObtenerUsuarioActual()
        {
            // CAMBIO ERP 2026-05-11: Trazabilidad real de usuario
            var claimId = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 999;
        }
    }
}
