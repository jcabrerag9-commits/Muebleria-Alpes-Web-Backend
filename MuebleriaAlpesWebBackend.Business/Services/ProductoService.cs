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
        private readonly IPrecioService _precioService;
        private readonly ILogger<ProductoService> _logger;
        private const int MONEDA_BASE = 1; // Quetzales por defecto

        public ProductoService(IProductoRepository productoRepository, IPrecioService precioService, ILogger<ProductoService> logger)
        {
            _productoRepository = productoRepository;
            _precioService = precioService;
            _logger = logger;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            _logger.LogInformation("[SERVICE] Solicitando todos los productos con carga masiva de precios");
            var productos = await _productoRepository.GetAllAsync();

            try
            {
                // CARGA MASIVA: Traemos todos los precios vigentes en una sola llamada
                var preciosMasivos = await _precioService.GetPreciosVigentesMasivoAsync(MONEDA_BASE);
                var diccionarioPrecios = preciosMasivos.ToDictionary(p => p.ProductoId);

                _logger.LogInformation("[SERVICE] Enriqueciendo {Count} productos con {PriceCount} precios encontrados", productos.Count(), diccionarioPrecios.Count);

                foreach (var prod in productos)
                {
                    if (diccionarioPrecios.TryGetValue(prod.Id, out var precio))
                    {
                        prod.PrecioVigente = precio.Precio;
                        prod.PrecioOferta = precio.PrecioOferta;
                        
                        // Si el precio de oferta es igual al vigente, no es oferta real
                        if (prod.PrecioOferta == prod.PrecioVigente) prod.PrecioOferta = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SERVICE] Falló la carga masiva de precios. Los productos se mostrarán sin precio.");
            }

            return productos;
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            _logger.LogInformation("[SERVICE] Buscando producto por ID: {Id}", id);
            var prod = await _productoRepository.GetByIdAsync(id);
            
            if (prod != null)
            {
                try 
                {
                    prod.PrecioVigente = await _precioService.GetPrecioVigenteAsync(prod.Id, MONEDA_BASE);
                    prod.PrecioOferta = await _precioService.GetPrecioFinalAsync(prod.Id, MONEDA_BASE);
                    if (prod.PrecioOferta == prod.PrecioVigente) prod.PrecioOferta = null;
                }
                catch (Exception ex) 
                {
                    _logger.LogWarning("[SERVICE] Error al cargar precio en detalle para {Id}: {Msg}", id, ex.Message);
                }
            }
            
            return prod;
        }

        public async Task CreateAsync(Producto producto)
        {
            _logger.LogInformation("[SERVICE] Iniciando creación de producto: {Nombre}", producto.Nombre);
            
            // 1. Crear producto base
            await _productoRepository.CreateAsync(producto);

            // 2. Registrar precio (si se proporcionó)
            if (producto.PrecioVigente.HasValue && producto.PrecioVigente > 0)
            {
                try
                {
                    _logger.LogInformation("[SERVICE] Registrando precio inicial para Producto {Id}", producto.Id);
                    await _precioService.RegistrarPrecioAsync(new PrecioProducto
                    {
                        ProductoId = producto.Id,
                        MonedaId = MONEDA_BASE,
                        Precio = producto.PrecioVigente.Value,
                        PrecioOferta = producto.PrecioOferta,
                        Tipo = "REGULAR",
                        FechaInicio = DateTime.Now,
                        Estado = "ACTIVO"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SERVICE] Error al registrar precio para {Id}. El producto existe pero sin precio.", producto.Id);
                }
            }
        }

        public async Task UpdateAsync(Producto producto)
        {
            _logger.LogInformation("[SERVICE] Iniciando actualización de Producto {Id}", producto.Id);
            
            // 1. Actualizar datos base
            await _productoRepository.UpdateAsync(producto);

            // 2. Si viene un nuevo precio, lo registramos (La lógica de PrecioService se encarga de invalidar el anterior)
            if (producto.PrecioVigente.HasValue && producto.PrecioVigente > 0)
            {
                try
                {
                    _logger.LogInformation("[SERVICE] Detectado cambio de precio en actualización para {Id}", producto.Id);
                    await _precioService.RegistrarPrecioAsync(new PrecioProducto
                    {
                        ProductoId = producto.Id,
                        MonedaId = MONEDA_BASE,
                        Precio = producto.PrecioVigente.Value,
                        PrecioOferta = producto.PrecioOferta,
                        Tipo = "REGULAR",
                        FechaInicio = DateTime.Now,
                        Estado = "ACTIVO"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SERVICE] Error al actualizar precio para {Id}", producto.Id);
                }
            }
        }

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
