using MuebleriaAlpesWebBackend.Domain.Entities;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IBannerRepository
    {
        // Banners
        Task<IEnumerable<Banner>> GetAllAsync(string? estado = null, string? posicion = null);
        Task<Banner?> GetByIdAsync(long id);
        Task<IEnumerable<Banner>> GetVigentesAsync(string? posicion = null);
        Task<long> CreateAsync(Banner banner);
        Task<bool> UpdateAsync(Banner banner);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);

        // Clicks
        Task<long> RegistrarClickAsync(ClickBanner click);
        Task<IEnumerable<ClickBanner>> GetClicksByBannerAsync(long bannerId);
        Task<int> GetTotalClicksAsync(long bannerId);
    }
}