using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ContenidoService : IContenidoService
    {
        private readonly IContenidoRepository _contenidoRepository;

        public ContenidoService(IContenidoRepository contenidoRepository)
        {
            _contenidoRepository = contenidoRepository;
        }

        public async Task<IEnumerable<ProductoImagen>> GetImagenesByProductoIdAsync(int productoId) => await _contenidoRepository.GetImagenesByProductoIdAsync(productoId);
        public async Task<int> CreateImagenAsync(ProductoImagen imagen) => await _contenidoRepository.CreateImagenAsync(imagen);
        public async Task UpdateImagenAsync(ProductoImagen imagen) => await _contenidoRepository.UpdateImagenAsync(imagen);
        public async Task SetImagenPrincipalAsync(int productoId, int imagenId) => await _contenidoRepository.SetImagenPrincipalAsync(productoId, imagenId);
        public async Task DeleteImagenAsync(int imagenId) => await _contenidoRepository.DeleteImagenAsync(imagenId);
        public async Task UpsertTraduccionAsync(ProductoTraduccion traduccion) => await _contenidoRepository.UpsertTraduccionAsync(traduccion);
    }
}
