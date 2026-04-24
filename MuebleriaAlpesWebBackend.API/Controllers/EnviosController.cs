using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.DTOs.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnviosController : ControllerBase
    {
        private readonly IEnviosService _enviosService;

        public EnviosController(IEnviosService enviosService)
        {
            _enviosService = enviosService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEnvio([FromBody] RegistrarEnvioRequest request)
        {
            try
            {
                var envioId = await _enviosService.RegistrarEnvioAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Envío registrado correctamente.",
                    envioId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarEnvio([FromBody] ActualizarEnvioRequest request)
        {
            try
            {
                var resultado = await _enviosService.ActualizarEnvioAsync(request);

                return Ok(new
                {
                    success = resultado,
                    message = "Envío actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPatch("estado")]
        public async Task<IActionResult> CambiarEstadoEnvio([FromBody] CambiarEstadoEnvioRequest request)
        {
            try
            {
                var resultado = await _enviosService.CambiarEstadoEnvioAsync(request);

                return Ok(new
                {
                    success = resultado,
                    message = "Estado del envío actualizado correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPatch("confirmar-entrega")]
        public async Task<IActionResult> ConfirmarEntregaEnvio([FromBody] ConfirmarEntregaEnvioRequest request)
        {
            try
            {
                var resultado = await _enviosService.ConfirmarEntregaEnvioAsync(request);

                return Ok(new
                {
                    success = resultado,
                    message = "Entrega del envío confirmada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPatch("orden/{ordenVentaId:int}/marcar-enviada")]
        public async Task<IActionResult> MarcarOrdenEnviada(int ordenVentaId)
        {
            try
            {
                var resultado = await _enviosService.MarcarOrdenEnviadaAsync(ordenVentaId);

                return Ok(new
                {
                    success = resultado,
                    message = "Orden marcada como enviada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{envioId:int}")]
        public async Task<IActionResult> ObtenerEnvio(int envioId)
        {
            try
            {
                var envio = await _enviosService.ObtenerEnvioAsync(envioId);

                if (envio == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontró el envío."
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = envio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("orden/{ordenVentaId:int}")]
        public async Task<IActionResult> ListarEnviosPorOrden(int ordenVentaId)
        {
            try
            {
                var envios = await _enviosService.ListarEnviosPorOrdenAsync(ordenVentaId);

                return Ok(new
                {
                    success = true,
                    data = envios
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("estado/{estado}")]
        public async Task<IActionResult> ListarEnviosPorEstado(string estado)
        {
            try
            {
                var envios = await _enviosService.ListarEnviosPorEstadoAsync(estado);

                return Ok(new
                {
                    success = true,
                    data = envios
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("guia/{numeroGuia}")]
        public async Task<IActionResult> BuscarEnvioPorGuia(string numeroGuia)
        {
            try
            {
                var envio = await _enviosService.BuscarEnvioPorGuiaAsync(numeroGuia);

                if (envio == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontró un envío con esa guía."
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = envio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}