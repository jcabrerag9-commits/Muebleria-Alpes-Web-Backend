using MuebleriaAlpesWebBackend.Domain.DTOs.Devoluciones;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IDevolucionService
    {
        Task<IEnumerable<DevolucionListDto>> GetAllAsync(string? estado = null, long? clienteId = null);
        Task<DevolucionResponseDto?> GetByIdAsync(long id);
        Task<DevolucionResponseDto?> GetByRmaAsync(string numeroRma);
        Task<IEnumerable<DevolucionListDto>> GetByOrdenVentaAsync(long ordenVentaId);
        Task<IEnumerable<DevolucionListDto>> GetByClienteAsync(long clienteId);

        Task<DevolucionResponseDto> CreateAsync(DevolucionCreateDto dto);
        Task<DevolucionResponseDto?> CambiarEstadoAsync(long id, DevolucionUpdateEstadoDto dto);
        Task<bool> DeleteAsync(long id);

        Task<IEnumerable<CategoriaDevolucionResponseDto>> GetCategoriasAsync(string? estado = null);
        Task<CategoriaDevolucionResponseDto?> GetCategoriaByIdAsync(long id);
    }
}
