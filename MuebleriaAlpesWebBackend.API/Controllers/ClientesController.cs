using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _clienteService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var perfil = await _clienteService.GetPerfilAsync(id);
            if (perfil == null) return NotFound();
            return Ok(perfil);
        }

        [HttpPost("registrar-completo")]
        public async Task<IActionResult> Registrar([FromBody] RegistroClienteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _clienteService.RegistrarClienteAsync(request.Cliente, request.Email, request.Telefono);
            return Ok(new { id, mensaje = "Cliente registrado con éxito" });
        }

        [HttpPost("{id}/emails")]
        public async Task<IActionResult> AddEmail(int id, [FromBody] ClienteEmail email)
        {
            email.ClienteId = id;
            var emailId = await _clienteService.AddEmailAsync(email);
            return Ok(new { id = emailId });
        }

        [HttpPost("{id}/direcciones")]
        public async Task<IActionResult> AddDireccion(int id, [FromBody] ClienteDireccion direccion)
        {
            direccion.ClienteId = id;
            var dirId = await _clienteService.AddDireccionAsync(direccion);
            return Ok(new { id = dirId });
        }

        [HttpPut("{id}/preferencias")]
        public async Task<IActionResult> UpdatePrefs(int id, [FromBody] ClientePreferencia pref)
        {
            pref.ClienteId = id;
            await _clienteService.UpdatePreferenciasAsync(pref);
            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string estado, [FromQuery] string motivo, [FromQuery] int? usuarioId)
        {
            await _clienteService.ChangeStatusAsync(id, estado, motivo, usuarioId);
            return NoContent();
        }
    }

    public class RegistroClienteRequest
    {
        [Required]
        public Cliente Cliente { get; set; }
        public ClienteEmail Email { get; set; }
        public ClienteTelefono Telefono { get; set; }
    }
}
