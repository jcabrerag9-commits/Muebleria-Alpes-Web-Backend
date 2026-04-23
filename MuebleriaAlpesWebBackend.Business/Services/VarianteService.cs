using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class VarianteService : IVarianteService
    {
        private readonly IVarianteRepository _varianteRepository;

        public VarianteService(IVarianteRepository varianteRepository)
        {
            _varianteRepository = varianteRepository;
        }

        public async Task<IEnumerable<ProductoVariante>> GetByProductoIdAsync(int productoId) => await _varianteRepository.GetByProductoIdAsync(productoId);
        public async Task<int> CreateAsync(ProductoVariante variante) => await _varianteRepository.CreateAsync(variante);
        public async Task UpdateAsync(ProductoVariante variante) => await _varianteRepository.UpdateAsync(variante);
        public async Task ChangeStatusAsync(int id, string estado) => await _varianteRepository.ChangeStatusAsync(id, estado);
        public async Task AsignarAtributoAsync(int varianteId, int atributoValorId) => await _varianteRepository.AsignarAtributoAsync(varianteId, atributoValorId);
        public async Task QuitarAtributoAsync(int varianteId, int atributoValorId) => await _varianteRepository.QuitarAtributoAsync(varianteId, atributoValorId);
    }
}
