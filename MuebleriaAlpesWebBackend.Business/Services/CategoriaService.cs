using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repo;

        public CategoriaService(ICategoriaRepository repo) => _repo = repo;

        public Task<IEnumerable<Categoria>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Categoria?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<IEnumerable<ProductoEnCategoria>> GetProductosByCategoriaAsync(int categoriaId)
            => _repo.GetProductosByCategoriaAsync(categoriaId);
        public Task<IEnumerable<Categoria>> GetCategoriasByProductoAsync(int productoId)
            => _repo.GetCategoriasByProductoAsync(productoId);
        public Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string codigo, string nombre, string? descripcion)
            => _repo.CreateAsync(codigo, nombre, descripcion);
        public Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion)
            => _repo.UpdateAsync(id, nombre, descripcion);
        public Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
