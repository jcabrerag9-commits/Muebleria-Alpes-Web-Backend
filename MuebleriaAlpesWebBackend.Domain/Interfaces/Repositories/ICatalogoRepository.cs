using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ICatalogoRepository
    {
        Task<IEnumerable<CatalogoItemDTO>> ListarTiposMuebleAsync();
    }
}
