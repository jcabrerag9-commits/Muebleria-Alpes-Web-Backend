using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet("db")]
        public async Task<IActionResult> ProbarConexion()
        {
            try
            {
                var resultado = await _testService.ProbarConexionAsync();

                return Ok(new
                {
                    mensaje = "Conexión exitosa con Oracle",
                    resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al conectar con Oracle",
                    detalle = ex.Message
                });
            }
        }
    }
}
