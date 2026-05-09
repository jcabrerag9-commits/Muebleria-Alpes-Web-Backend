using MuebleriaAlpesWebBackend.Domain.Entities;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IDevolucionRepository
    {
        // ── Categorías — CRUD completo ────────────────────────────────────────
        Task<IEnumerable<CategoriaDevolucion>> GetCategoriasAsync(string? estado = null);
        Task<CategoriaDevolucion?> GetCategoriaByIdAsync(long id);
        Task<CategoriaDevolucion?> GetCategoriaByCodigoAsync(string codigo);
        Task<long> CreateCategoriaAsync(CategoriaDevolucion categoria);
        Task<bool> UpdateCategoriaAsync(CategoriaDevolucion categoria);
        Task<bool> DeleteCategoriaAsync(long id);
        Task<bool> CategoriaExistsAsync(long id);
        Task<bool> CodigoCategoriaExistsAsync(string codigo, long? excludeId = null);
        Task<bool> CategoriaEnUsoAsync(long id);

        // ── Devoluciones ──────────────────────────────────────────────────────
        Task<IEnumerable<Devolucion>> GetAllAsync(string? estado = null, long? clienteId = null);
        Task<Devolucion?> GetByIdAsync(long id);
        Task<Devolucion?> GetByRmaAsync(string numeroRma);
        Task<IEnumerable<Devolucion>> GetByOrdenVentaAsync(long ordenVentaId);
        Task<IEnumerable<Devolucion>> GetByClienteAsync(long clienteId);
        Task<long> CreateAsync(Devolucion devolucion);
        Task<bool> UpdateEstadoAsync(long id, string nuevoEstado);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<string> GenerarNumeroRmaAsync();

        // ── Detalles ──────────────────────────────────────────────────────────
        Task<IEnumerable<DevolucionDetalle>> GetDetallesByDevolucionAsync(long devolucionId);
        Task AddDetallesAsync(IEnumerable<DevolucionDetalle> detalles);
    }
}