using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _repo;

        public ColorService(IColorRepository repo) => _repo = repo;

        public Task<IEnumerable<Color>> GetAllAsync()  => _repo.GetAllAsync();
        public Task<Color?> GetByIdAsync(int id)       => _repo.GetByIdAsync(id);

        public Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string nombre, string hexColor, string? descripcion)
            => _repo.CreateAsync(nombre, hexColor, descripcion);

        public Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string hexColor, string? descripcion)
            => _repo.UpdateAsync(id, nombre, hexColor, descripcion);

        public Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
