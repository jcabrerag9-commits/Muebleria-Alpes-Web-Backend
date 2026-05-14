using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet("tipos-mueble")]
        public async Task<ActionResult<InventarioResponse<IEnumerable<CatalogoItemDTO>>>> GetTiposMueble()
        {
            System.Console.WriteLine("[CatalogosController] Petición GET tipos-mueble recibida.");
            var tipos = await _catalogoService.ListarTiposMuebleAsync();
            
            var response = new InventarioResponse<IEnumerable<CatalogoItemDTO>>
            {
                Resultado = "EXITO",
                Mensaje = "Tipos de mueble recuperados exitosamente",
                Data = tipos
            };
            
            System.Console.WriteLine($"[CatalogosController] Enviando {((List<CatalogoItemDTO>)tipos).Count} tipos al cliente.");
            return Ok(response);
        }
    }
}
