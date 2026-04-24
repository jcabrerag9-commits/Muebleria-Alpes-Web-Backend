using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguridadController : ControllerBase
    {
        private readonly ISeguridadService _seguridadService;

        public SeguridadController(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
        }

        [HttpPost("usuarios")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request)
        {
            try
            {
                var response = await _seguridadService.CrearUsuarioAsync(request);
                return Ok(new { success = true, message = "Usuario creado correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("usuarios")]
        public async Task<IActionResult> ActualizarUsuario([FromBody] ActualizarUsuarioRequest request)
        {
            try
            {
                var ok = await _seguridadService.ActualizarUsuarioAsync(request);
                return Ok(new { success = ok, message = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("usuarios/password")]
        public async Task<IActionResult> CambiarPasswordUsuario([FromBody] CambiarPasswordUsuarioRequest request)
        {
            try
            {
                var ok = await _seguridadService.CambiarPasswordUsuarioAsync(request);
                return Ok(new { success = ok, message = "Password actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("usuarios/bloquear")]
        public async Task<IActionResult> BloquearUsuario([FromBody] BloquearUsuarioRequest request)
        {
            try
            {
                var ok = await _seguridadService.BloquearUsuarioAsync(request);
                return Ok(new { success = ok, message = "Usuario bloqueado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("usuarios/desbloquear")]
        public async Task<IActionResult> DesbloquearUsuario([FromBody] DesbloquearUsuarioRequest request)
        {
            try
            {
                var ok = await _seguridadService.DesbloquearUsuarioAsync(request);
                return Ok(new { success = ok, message = "Usuario desbloqueado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("usuarios/logico")]
        public async Task<IActionResult> EliminarUsuarioLogico([FromBody] EliminarUsuarioLogicoRequest request)
        {
            try
            {
                var ok = await _seguridadService.EliminarUsuarioLogicoAsync(request);
                return Ok(new { success = ok, message = "Usuario inactivado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CrearRol([FromBody] CrearRolRequest request)
        {
            try
            {
                var response = await _seguridadService.CrearRolAsync(request);
                return Ok(new { success = true, message = "Rol creado correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("roles")]
        public async Task<IActionResult> ActualizarRol([FromBody] ActualizarRolRequest request)
        {
            try
            {
                var ok = await _seguridadService.ActualizarRolAsync(request);
                return Ok(new { success = ok, message = "Rol actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("roles/logico")]
        public async Task<IActionResult> EliminarRolLogico([FromBody] EliminarRolLogicoRequest request)
        {
            try
            {
                var ok = await _seguridadService.EliminarRolLogicoAsync(request);
                return Ok(new { success = ok, message = "Rol inactivado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("permisos")]
        public async Task<IActionResult> CrearPermiso([FromBody] CrearPermisoRequest request)
        {
            try
            {
                var response = await _seguridadService.CrearPermisoAsync(request);
                return Ok(new { success = true, message = "Permiso creado correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("permisos")]
        public async Task<IActionResult> ActualizarPermiso([FromBody] ActualizarPermisoRequest request)
        {
            try
            {
                var ok = await _seguridadService.ActualizarPermisoAsync(request);
                return Ok(new { success = ok, message = "Permiso actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("permisos/logico")]
        public async Task<IActionResult> EliminarPermisoLogico([FromBody] EliminarPermisoLogicoRequest request)
        {
            try
            {
                var ok = await _seguridadService.EliminarPermisoLogicoAsync(request);
                return Ok(new { success = ok, message = "Permiso inactivado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("usuarios/roles")]
        public async Task<IActionResult> AsignarRolUsuario([FromBody] AsignarRolUsuarioRequest request)
        {
            try
            {
                var response = await _seguridadService.AsignarRolUsuarioAsync(request);
                return Ok(new { success = true, message = "Rol asignado al usuario correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("usuarios/roles/quitar")]
        public async Task<IActionResult> QuitarRolUsuario([FromBody] QuitarRolUsuarioRequest request)
        {
            try
            {
                var ok = await _seguridadService.QuitarRolUsuarioAsync(request);
                return Ok(new { success = ok, message = "Rol retirado del usuario correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("roles/permisos")]
        public async Task<IActionResult> AsignarPermisoRol([FromBody] AsignarPermisoRolRequest request)
        {
            try
            {
                var response = await _seguridadService.AsignarPermisoRolAsync(request);
                return Ok(new { success = true, message = "Permiso asignado al rol correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("roles/permisos/quitar")]
        public async Task<IActionResult> QuitarPermisoRol([FromBody] QuitarPermisoRolRequest request)
        {
            try
            {
                var ok = await _seguridadService.QuitarPermisoRolAsync(request);
                return Ok(new { success = ok, message = "Permiso retirado del rol correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("bitacora-acceso")]
        public async Task<IActionResult> RegistrarBitacoraAcceso([FromBody] RegistrarBitacoraAccesoRequest request)
        {
            try
            {
                var response = await _seguridadService.RegistrarBitacoraAccesoAsync(request);
                return Ok(new { success = true, message = "Bitácora registrada correctamente.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}