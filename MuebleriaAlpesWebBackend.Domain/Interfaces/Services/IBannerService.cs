using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerResponseDto>> GetAllAsync(string? estado = null, string? posicion = null);
        Task<BannerResponseDto?> GetByIdAsync(long id);
        Task<IEnumerable<BannerResponseDto>> GetVigentesAsync(string? posicion = null);
        Task<BannerEstadisticasDto?> GetEstadisticasAsync(long id);

        Task<BannerResponseDto> CreateAsync(BannerCreateDto dto);
        Task<BannerResponseDto?> UpdateAsync(long id, BannerUpdateDto dto);
        Task<bool> DeleteAsync(long id);

        Task<ClickBannerResponseDto> RegistrarClickAsync(long bannerId, ClickBannerCreateDto dto);
    }
}