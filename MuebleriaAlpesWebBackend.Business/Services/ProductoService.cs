using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(IProductoRepository productoRepository, ILogger<ProductoService> logger)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            _logger.LogInformation("[SERVICE] Solicitando todos los productos");
            return await _productoRepository.GetAllAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            _logger.LogInformation("[SERVICE] Buscando producto por ID: {Id}", id);
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Producto producto)
        {
            _logger.LogInformation("[SERVICE] Validando creación de producto: {Nombre}", producto.Nombre);
            
            if (string.IsNullOrWhiteSpace(producto.Nombre) || producto.Nombre.Length < 3)
            {
                _logger.LogWarning("[SERVICE] Validación fallida: Nombre demasiado corto");
                throw new ArgumentException("El nombre del producto debe tener al menos 3 caracteres.");
            }

            await _productoRepository.CreateAsync(producto);
            _logger.LogInformation("[SERVICE] Producto creado exitosamente con ID asignado: {Id}", producto.Id);
        }
        public async Task UpdateAsync(Producto producto) => await _productoRepository.UpdateAsync(producto);
        public async Task ChangeStatusAsync(int id, string estado) => await _productoRepository.ChangeStatusAsync(id, estado);
        public async Task DeleteLogicoAsync(int id) => await _productoRepository.DeleteLogicoAsync(id);
        public async Task UpsertDimensionAsync(DimensionProducto dimension) => await _productoRepository.UpsertDimensionAsync(dimension);
        public async Task AsignarCategoriaAsync(int productoId, int categoriaId) => await _productoRepository.AsignarCategoriaAsync(productoId, categoriaId);
        public async Task QuitarCategoriaAsync(int productoId, int categoriaId) => await _productoRepository.QuitarCategoriaAsync(productoId, categoriaId);
        public async Task AsignarColeccionAsync(int productoId, int coleccionId) => await _productoRepository.AsignarColeccionAsync(productoId, coleccionId);
        public async Task QuitarColeccionAsync(int productoId, int coleccionId) => await _productoRepository.QuitarColeccionAsync(productoId, coleccionId);
        public async Task AsignarMaterialAsync(int productoId, int materialId) => await _productoRepository.AsignarMaterialAsync(productoId, materialId);
        public async Task QuitarMaterialAsync(int productoId, int materialId) => await _productoRepository.QuitarMaterialAsync(productoId, materialId);
        public async Task AsignarColorAsync(int productoId, int colorId) => await _productoRepository.AsignarColorAsync(productoId, colorId);
        public async Task QuitarColorAsync(int productoId, int colorId) => await _productoRepository.QuitarColorAsync(productoId, colorId);
        public async Task<int> CreateResenaAsync(ResenaProducto resena) => await _productoRepository.CreateResenaAsync(resena);
        public async Task AprobarResenaAsync(int resenaId) => await _productoRepository.AprobarResenaAsync(resenaId);
    }
}
