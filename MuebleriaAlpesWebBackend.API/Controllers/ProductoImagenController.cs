using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoImagenController : ControllerBase
    {
        private readonly IProductoImagenService _service;
        private readonly ILogger<ProductoImagenController> _logger;
        private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] ALLOWED_MIME_TYPES = { "image/jpeg", "image/png", "image/webp" };

        public ProductoImagenController(IProductoImagenService service, ILogger<ProductoImagenController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Sube una imagen de producto al servidor (Oracle BLOB).
        /// Límite máximo: 10MB. Formatos permitidos: JPG, PNG, WEBP.
        /// </summary>
        /// <param name="request">Datos del producto y archivo.</param>
        /// <returns>ID de la imagen generada.</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] UploadProductoImagenRequest request)
        {
            if (request.Archivo == null || request.Archivo.Length == 0)
                return BadRequest(new ApiResponse<int> { Success = false, Message = "Archivo vacío." });

            string extension = Path.GetExtension(request.Archivo.FileName).ToLower();
            if (!ALLOWED_EXTENSIONS.Contains(extension) || !ALLOWED_MIME_TYPES.Contains(request.Archivo.ContentType.ToLower()))
                return BadRequest(new ApiResponse<int> { Success = false, Message = "Formato no permitido." });

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await request.Archivo.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var response = await _service.SubirImagenAsync(
                request.ProductoId,
                fileBytes,
                request.Archivo.FileName,
                request.Archivo.ContentType,
                request.Archivo.Length,
                request.UrlOpcional,
                request.Tipo,
                request.Orden
            );

            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene el archivo binario de una imagen por su ID.
        /// </summary>
        /// <param name="id">ID de la imagen.</param>
        /// <returns>File stream.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("[API-IMAGEN] GET /api/ProductoImagen/{Id} solicitado", id);
            var imagen = await _service.ObtenerImagenAsync(id);
            if (imagen == null || imagen.Archivo == null) 
            {
                _logger.LogWarning("[API-IMAGEN] Imagen {Id} no encontrada o sin binario.", id);
                return NotFound();
            }

            _logger.LogInformation("[API-IMAGEN] Sirviendo imagen {Id}. Filename: {Name}, Size: {Size} bytes", id, imagen.NombreArchivo, imagen.Archivo.Length);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{imagen.NombreArchivo}\"");
            return File(imagen.Archivo, imagen.ContentType ?? "image/jpeg");
        }

        /// <summary>
        /// Obtiene el listado de metadatos de imágenes para un producto específico.
        /// </summary>
        /// <param name="productoId">ID del producto.</param>
        /// <returns>Lista de imágenes activas.</returns>
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> GetByProducto(int productoId)
        {
            _logger.LogInformation("[API-IMAGEN] GET listado para producto {Id} solicitado", productoId);
            var imagenes = await _service.ListarPorProductoAsync(productoId);
            _logger.LogInformation("[API-IMAGEN] Se retornan {Count} imágenes para producto {Id}", imagenes?.Count() ?? 0, productoId);
            return Ok(new ApiResponse<IEnumerable<ProductoImagenListadoDTO>>
            {
                Success = true,
                Message = "Listado obtenido",
                Data = imagenes
            });
        }

        /// <summary>
        /// Obtiene la imagen principal de un producto.
        /// </summary>
        /// <param name="productoId">ID del producto.</param>
        /// <returns>File stream de la imagen principal.</returns>
        [HttpGet("producto/{productoId}/principal")]
        public async Task<IActionResult> GetPrincipal(int productoId)
        {
            _logger.LogInformation("[API-IMAGEN] GET principal para producto {Id} solicitado", productoId);
            var imagen = await _service.ObtenerPrincipalPorProductoAsync(productoId);
            
            if (imagen == null || imagen.Archivo == null) 
            {
                _logger.LogWarning("[API-IMAGEN] No hay imagen principal para producto {Id}", productoId);
                return NotFound();
            }

            _logger.LogInformation("[API-IMAGEN] Sirviendo principal para producto {Id}. Size: {Size} bytes", productoId, imagen.Archivo.Length);
            return File(imagen.Archivo, imagen.ContentType ?? "image/jpeg");
        }

        /// <summary>
        /// Realiza la eliminación lógica (soft delete) de una imagen.
        /// </summary>
        /// <param name="id">ID de la imagen.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.EliminarImagenAsync(id);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }

    public class UploadProductoImagenRequest
    {
        public int ProductoId { get; set; }
        public IFormFile Archivo { get; set; } = null!;
        public string? Tipo { get; set; }
        public int Orden { get; set; } = 1;
        public string? UrlOpcional { get; set; }
    }
}
