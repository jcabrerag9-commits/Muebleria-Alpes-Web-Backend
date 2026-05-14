using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IFinanzasService
    {
        Task<IEnumerable<HistorialFinancieroDTO>> ObtenerHistorialFinancieroAsync(HistorialFiltroRequest request, System.Threading.CancellationToken ct = default);
    }
}
