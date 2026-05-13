using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _clienteService.RegistrarClienteAsync(cliente);
            return Ok();
        }

        [HttpPost("{clienteId}/emails")]
        public async Task<IActionResult> AddEmail(int clienteId, [FromBody] ClienteEmail email)
        {
            email.ClienteId = clienteId;
            await _clienteService.AddEmailAsync(email);
            return Ok();
        }

        [HttpPost("{clienteId}/telefonos")]
        public async Task<IActionResult> AddTelefono(int clienteId, [FromBody] ClienteTelefono telefono)
        {
            telefono.ClienteId = clienteId;
            await _clienteService.AddTelefonoAsync(telefono);
            return Ok();
        }

        [HttpPost("{clienteId}/direcciones")]
        public async Task<IActionResult> AddDireccion(int clienteId, [FromBody] ClienteDireccion direccion)
        {
            direccion.ClienteId = clienteId;
            await _clienteService.AddDireccionAsync(direccion);
            return Ok();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _clienteService.EliminarLogicoAsync(id, "Eliminación desde sistema web", null);
                return Ok(new { mensaje = "Cliente eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

}
