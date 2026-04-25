using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IUbicacionRepository _ubicacionRepository;

        public UbicacionService(IUbicacionRepository ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
        }

        public async Task<IEnumerable<Pais>> GetPaisesAsync() => await _ubicacionRepository.ListarPaisesAsync();

        public async Task<int> CreatePaisAsync(Pais pais)
        {
            if (string.IsNullOrWhiteSpace(pais.Codigo)) throw new ArgumentException("El código de país es obligatorio.");
            if (string.IsNullOrWhiteSpace(pais.Nombre)) throw new ArgumentException("El nombre del país es obligatorio.");
            
            return await _ubicacionRepository.CrearPaisAsync(pais);
        }

        public async Task<IEnumerable<Departamento>> GetDepartamentosAsync() => await _ubicacionRepository.ListarDepartamentosAsync();

        public async Task<int> CreateDepartamentoAsync(Departamento depto)
        {
            if (depto.PaisId <= 0) throw new ArgumentException("Debe asociar un país válido.");
            return await _ubicacionRepository.CrearDepartamentoAsync(depto);
        }

        public async Task<IEnumerable<Ciudad>> GetCiudadesAsync() => await _ubicacionRepository.ListarCiudadesAsync();

        public async Task<int> CreateCiudadAsync(Ciudad ciudad)
        {
            if (ciudad.DepartamentoId <= 0) throw new ArgumentException("Debe asociar un departamento válido.");
            return await _ubicacionRepository.CrearCiudadAsync(ciudad);
        }

        public async Task<IEnumerable<Idioma>> GetIdiomasAsync() => await _ubicacionRepository.ListarIdiomasAsync();

        public async Task<int> CreateIdiomaAsync(Idioma idioma)
        {
            if (string.IsNullOrWhiteSpace(idioma.Codigo)) throw new ArgumentException("El código de idioma es obligatorio.");
            return await _ubicacionRepository.CrearIdiomaAsync(idioma);
        }

        public async Task<IEnumerable<Moneda>> GetMonedasAsync() => await _ubicacionRepository.ListarMonedasAsync();

        public async Task<int> CreateMonedaAsync(Moneda moneda)
        {
            if (string.IsNullOrWhiteSpace(moneda.Simbolo)) throw new ArgumentException("El símbolo de moneda es obligatorio.");
            return await _ubicacionRepository.CrearMonedaAsync(moneda);
        }
    }
}
