using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IColorService
    {
        Task<IEnumerable<Color>> GetAllAsync();
        Task<Color?> GetByIdAsync(int id);
        Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string nombre, string hexColor, string? descripcion);
        Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string hexColor, string? descripcion);
        Task<(bool Ok, string Mensaje)> DeleteAsync(int id);
    }
}
