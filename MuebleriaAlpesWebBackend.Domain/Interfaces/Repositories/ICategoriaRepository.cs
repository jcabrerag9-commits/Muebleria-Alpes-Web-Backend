using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoEnCategoria>> GetProductosByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Categoria>> GetCategoriasByProductoAsync(int productoId);
        Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string codigo, string nombre, string? descripcion);
        Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion);
        Task<(bool Ok, string Mensaje)> DeleteAsync(int id);
    }
}
