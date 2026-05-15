using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _repo;

        public MaterialService(IMaterialRepository repo) => _repo = repo;

        public Task<IEnumerable<Material>> GetAllAsync()  => _repo.GetAllAsync();
        public Task<Material?> GetByIdAsync(int id)       => _repo.GetByIdAsync(id);

        public Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string nombre, string? descripcion)
            => _repo.CreateAsync(nombre, descripcion);

        public Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion)
            => _repo.UpdateAsync(id, nombre, descripcion);

        public Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
