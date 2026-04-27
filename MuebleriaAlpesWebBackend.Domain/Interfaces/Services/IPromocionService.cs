using MuebleriaAlpesWebBackend.Domain.DTOs.Promociones;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IPromocionService
    {
        Task<IEnumerable<PromocionListDto>> GetAllAsync(string? estado = null, string? tipo = null);
        Task<PromocionResponseDto?> GetByIdAsync(long id);
        Task<PromocionResponseDto?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<PromocionListDto>> GetVigentesAsync();

        Task<PromocionResponseDto> CreateAsync(PromocionCreateDto dto);
        Task<PromocionResponseDto?> UpdateAsync(long id, PromocionUpdateDto dto);
        Task<bool> DeleteAsync(long id);

        Task<PromocionProductoResponseDto> AddProductoAsync(long promocionId, PromocionProductoCreateDto dto);
        Task<bool> RemoveProductoAsync(long promocionId, long ppoId);
    }
}
