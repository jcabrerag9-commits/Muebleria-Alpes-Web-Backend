using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IVarianteService
    {
        Task<IEnumerable<ProductoVariante>> GetByProductoIdAsync(int productoId);
        Task<int> CreateAsync(ProductoVariante variante);
        Task UpdateAsync(ProductoVariante variante);
        Task ChangeStatusAsync(int id, string estado);
        Task AsignarAtributoAsync(int varianteId, int atributoValorId);
        Task QuitarAtributoAsync(int varianteId, int atributoValorId);
    }
}
