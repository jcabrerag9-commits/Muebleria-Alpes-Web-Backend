using MuebleriaAlpesWebBackend.Domain.DTOs.Devoluciones;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IDevolucionService
    {
        // ── Categorías — CRUD completo ────────────────────────────────────────
        Task<IEnumerable<CategoriaDevolucionResponseDto>> GetCategoriasAsync(string? estado = null);
        Task<CategoriaDevolucionResponseDto?> GetCategoriaByIdAsync(long id);
        Task<CategoriaDevolucionResponseDto> CreateCategoriaAsync(CategoriaDevolucionCreateDto dto);
        Task<CategoriaDevolucionResponseDto?> UpdateCategoriaAsync(long id, CategoriaDevolucionUpdateDto dto);
        Task<bool> DeleteCategoriaAsync(long id);

        // ── Devoluciones ──────────────────────────────────────────────────────
        Task<IEnumerable<DevolucionListDto>> GetAllAsync(string? estado = null, long? clienteId = null);
        Task<DevolucionResponseDto?> GetByIdAsync(long id);
        Task<DevolucionResponseDto?> GetByRmaAsync(string numeroRma);
        Task<IEnumerable<DevolucionListDto>> GetByOrdenVentaAsync(long ordenVentaId);
        Task<IEnumerable<DevolucionListDto>> GetByClienteAsync(long clienteId);
        Task<DevolucionResponseDto> CreateAsync(DevolucionCreateDto dto);
        Task<DevolucionResponseDto?> CambiarEstadoAsync(long id, DevolucionUpdateEstadoDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
