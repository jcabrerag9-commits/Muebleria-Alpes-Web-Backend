using MuebleriaAlpesWebBackend.Domain.Entities;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IPromocionRepository
    {
        // Consultas
        Task<IEnumerable<Promocion>> GetAllAsync(string? estado = null, string? tipo = null);
        Task<Promocion?> GetByIdAsync(long id);
        Task<Promocion?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Promocion>> GetVigentesAsync();

        // CRUD
        Task<long> CreateAsync(Promocion promocion);
        Task<bool> UpdateAsync(Promocion promocion);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<bool> CodigoExistsAsync(string codigo, long? excludeId = null);

        // PromocionProducto
        Task<IEnumerable<PromocionProducto>> GetProductosByPromocionAsync(long promocionId);
        Task<long> AddProductoAsync(PromocionProducto pp);
        Task<bool> RemoveProductoAsync(long ppoId);
    }
}
