using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task<int> CreateAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task ChangeStatusAsync(int id, string estado);
        Task DeleteLogicoAsync(int id);
        Task UpsertDimensionAsync(DimensionProducto dimension);
        Task AsignarCategoriaAsync(int productoId, int categoriaId);
        Task QuitarCategoriaAsync(int productoId, int categoriaId);
        Task AsignarColeccionAsync(int productoId, int coleccionId);
        Task QuitarColeccionAsync(int productoId, int coleccionId);
        Task AsignarMaterialAsync(int productoId, int materialId);
        Task QuitarMaterialAsync(int productoId, int materialId);
        Task AsignarColorAsync(int productoId, int colorId);
        Task QuitarColorAsync(int productoId, int colorId);
        Task<int> CreateResenaAsync(ResenaProducto resena);
        Task AprobarResenaAsync(int resenaId);
    }
}
