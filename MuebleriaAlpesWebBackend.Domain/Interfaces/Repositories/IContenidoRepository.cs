using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IContenidoRepository
    {
        Task<IEnumerable<ProductoImagen>> GetImagenesByProductoIdAsync(int productoId);
        Task<int> CreateImagenAsync(ProductoImagen imagen);
        Task UpdateImagenAsync(ProductoImagen imagen);
        Task SetImagenPrincipalAsync(int productoId, int imagenId);
        Task DeleteImagenAsync(int imagenId);
        
        Task UpsertTraduccionAsync(ProductoTraduccion traduccion);
    }
}
