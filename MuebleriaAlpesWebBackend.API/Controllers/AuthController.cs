using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Auth;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService    _authService;
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthService authService, IAuthRepository authRepository)
        {
            _authService    = authService;
            _authRepository = authRepository;
        }

        /// <summary>Registro de nuevo cliente.</summary>
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequestDto request)
        {
            var resultado = await _authService.RegistrarAsync(request);
            return resultado.Exitoso ? Ok(resultado) : BadRequest(resultado);
        }

        /// <summary>Login de usuario del sistema.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var resultado = await _authService.LoginAsync(request);
            return resultado.Exitoso ? Ok(resultado) : Unauthorized(resultado);
        }

        /// <summary>
        /// Diagnóstico de autenticación — solo para desarrollo.
        /// </summary>
        [HttpGet("diagnostico/{username}")]
        public async Task<IActionResult> Diagnostico(string username, [FromQuery] string password = "password")
        {
            var resultado = await _authRepository.DiagnosticoAsync(username, password);
            return Ok(resultado);
        }

        /// <summary>
        /// Busca o crea un ALP_CLIENTE para el usuario web identificado por username.
        /// POST api/auth/ensure-cliente → { clienteId: int }
        /// </summary>
        [HttpPost("ensure-cliente")]
        public async Task<IActionResult> EnsureCliente([FromBody] EnsureClienteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { mensaje = "Username requerido" });

            var clienteId = await _authService.EnsureClienteAsync(request.Username);
            if (clienteId == 0)
                return StatusCode(500, new { mensaje = "No se pudo crear/encontrar el cliente. Verifica ALP_TIPO_CLIENTE y ALP_TIPO_DOCUMENTO." });

            return Ok(new { clienteId });
        }
    }

    public record EnsureClienteRequest(string Username);
}
