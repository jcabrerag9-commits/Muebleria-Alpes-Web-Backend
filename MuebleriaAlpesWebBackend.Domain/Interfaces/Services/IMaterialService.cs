using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllAsync();
        Task<Material?> GetByIdAsync(int id);
        Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string nombre, string? descripcion);
        Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion);
        Task<(bool Ok, string Mensaje)> DeleteAsync(int id);
    }
}
