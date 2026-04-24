using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly IAutenticacionService _autenticacionService;

        public AutenticacionController(IAutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        [HttpPost("validar-login")]
        public async Task<IActionResult> ValidarLogin([FromBody] ValidarLoginRequest request)
        {
            try
            {
                var response = await _autenticacionService.ValidarLoginAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("iniciar-sesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionRequest request)
        {
            try
            {
                var response = await _autenticacionService.IniciarSesionAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("cerrar-sesion")]
        public async Task<IActionResult> CerrarSesion([FromBody] CerrarSesionRequest request)
        {
            try
            {
                var ok = await _autenticacionService.CerrarSesionAsync(request);
                return Ok(new { success = ok, message = "Sesión cerrada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("generar-token-recuperacion")]
        public async Task<IActionResult> GenerarTokenRecuperacion([FromBody] GenerarTokenRecuperacionRequest request)
        {
            try
            {
                var response = await _autenticacionService.GenerarTokenRecuperacionAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("marcar-token-usado")]
        public async Task<IActionResult> MarcarTokenUsado([FromBody] MarcarTokenRecuperacionUsadoRequest request)
        {
            try
            {
                var ok = await _autenticacionService.MarcarTokenRecuperacionUsadoAsync(request);
                return Ok(new { success = ok, message = "Token marcado como usado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sesion-activa/{usuarioId:int}")]
        public async Task<IActionResult> SesionActiva(int usuarioId)
        {
            try
            {
                var response = await _autenticacionService.SesionActivaAsync(usuarioId);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("token-recuperacion-valido/{token}")]
        public async Task<IActionResult> TokenRecuperacionValido(string token)
        {
            try
            {
                var response = await _autenticacionService.TokenRecuperacionValidoAsync(token);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("cliente-por-usuario/{usuarioId:int}")]
        public async Task<IActionResult> ObtenerClientePorUsuario(int usuarioId)
        {
            try
            {
                var response = await _autenticacionService.ObtenerClientePorUsuarioAsync(usuarioId);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}