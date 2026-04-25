using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IUbicacionRepository
    {
        // País
        Task<IEnumerable<Pais>> ListarPaisesAsync();
        Task<int> CrearPaisAsync(Pais pais);
        Task ActualizarPaisAsync(Pais pais);
        
        // Departamento
        Task<IEnumerable<Departamento>> ListarDepartamentosAsync();
        Task<int> CrearDepartamentoAsync(Departamento depto);
        
        // Ciudad
        Task<IEnumerable<Ciudad>> ListarCiudadesAsync();
        Task<int> CrearCiudadAsync(Ciudad ciudad);
        
        // Idioma
        Task<IEnumerable<Idioma>> ListarIdiomasAsync();
        Task<int> CrearIdiomaAsync(Idioma idioma);
        
        // Moneda
        Task<IEnumerable<Moneda>> ListarMonedasAsync();
        Task<int> CrearMonedaAsync(Moneda moneda);
    }
}
