using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface ICatalogoService
    {
        Task<IEnumerable<CatalogoItemDTO>> ListarTiposMuebleAsync();
    }
}
