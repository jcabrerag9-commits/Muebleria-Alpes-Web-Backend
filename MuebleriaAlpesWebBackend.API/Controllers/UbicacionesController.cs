using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UbicacionesController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionesController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        // --- PAÍSES ---
        [HttpGet("paises")]
        public async Task<IActionResult> GetPaises() => Ok(await _ubicacionService.GetPaisesAsync());

        [HttpPost("paises")]
        public async Task<IActionResult> CreatePais([FromBody] Pais pais)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _ubicacionService.CreatePaisAsync(pais);
            return Ok(new { id });
        }

        // --- DEPARTAMENTOS ---
        [HttpGet("departamentos")]
        public async Task<IActionResult> GetDepartamentos() => Ok(await _ubicacionService.GetDepartamentosAsync());

        [HttpPost("departamentos")]
        public async Task<IActionResult> CreateDepartamento([FromBody] Departamento depto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _ubicacionService.CreateDepartamentoAsync(depto);
            return Ok(new { id });
        }

        // --- CIUDADES ---
        [HttpGet("ciudades")]
        public async Task<IActionResult> GetCiudades() => Ok(await _ubicacionService.GetCiudadesAsync());

        [HttpPost("ciudades")]
        public async Task<IActionResult> CreateCiudad([FromBody] Ciudad ciudad)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _ubicacionService.CreateCiudadAsync(ciudad);
            return Ok(new { id });
        }

        // --- IDIOMAS ---
        [HttpGet("idiomas")]
        public async Task<IActionResult> GetIdiomas() => Ok(await _ubicacionService.GetIdiomasAsync());

        [HttpPost("idiomas")]
        public async Task<IActionResult> CreateIdioma([FromBody] Idioma idioma)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _ubicacionService.CreateIdiomaAsync(idioma);
            return Ok(new { id });
        }

        // --- MONEDAS ---
        [HttpGet("monedas")]
        public async Task<IActionResult> GetMonedas() => Ok(await _ubicacionService.GetMonedasAsync());

        [HttpPost("monedas")]
        public async Task<IActionResult> CreateMoneda([FromBody] Moneda moneda)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _ubicacionService.CreateMonedaAsync(moneda);
            return Ok(new { id });
        }
    }
}
