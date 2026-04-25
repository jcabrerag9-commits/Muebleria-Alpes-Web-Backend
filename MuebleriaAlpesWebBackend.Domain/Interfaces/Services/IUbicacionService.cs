using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IUbicacionService
    {
        Task<IEnumerable<Pais>> GetPaisesAsync();
        Task<int> CreatePaisAsync(Pais pais);
        
        Task<IEnumerable<Departamento>> GetDepartamentosAsync();
        Task<int> CreateDepartamentoAsync(Departamento depto);
        
        Task<IEnumerable<Ciudad>> GetCiudadesAsync();
        Task<int> CreateCiudadAsync(Ciudad ciudad);
        
        Task<IEnumerable<Idioma>> GetIdiomasAsync();
        Task<int> CreateIdiomaAsync(Idioma idioma);
        
        Task<IEnumerable<Moneda>> GetMonedasAsync();
        Task<int> CreateMonedaAsync(Moneda moneda);
    }
}
