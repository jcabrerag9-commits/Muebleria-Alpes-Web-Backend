using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class PrecioService : IPrecioService
    {
        private readonly IPrecioRepository _precioRepository;

        public PrecioService(IPrecioRepository precioRepository)
        {
            _precioRepository = precioRepository;
        }

        public async Task<int> RegistrarPrecioAsync(PrecioProducto precio)
        {
            // Validaciones de Negocio Senior
            if (precio.Precio <= 0)
                throw new ArgumentException("El precio debe ser un valor positivo.");

            if (precio.PrecioOferta.HasValue && precio.PrecioOferta >= precio.Precio)
                throw new ArgumentException("El precio de oferta debe ser menor al precio regular.");

            if (precio.FechaFin.HasValue && precio.FechaFin <= precio.FechaInicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

            return await _precioRepository.CreateAsync(precio);
        }

        public async Task ActualizarPrecioAsync(ActualizarPrecioRequest request)
        {
            if (request.NuevoPrecio.HasValue && request.NuevoPrecio <= 0)
                throw new ArgumentException("El nuevo precio debe ser positivo.");

            if (string.IsNullOrWhiteSpace(request.Motivo))
                throw new ArgumentException("Debe proporcionar un motivo para el cambio de precio.");

            await _precioRepository.UpdateAsync(request);
        }

        public async Task<decimal> GetPrecioVigenteAsync(int productoId, int monedaId)
        {
            if (productoId <= 0) throw new ArgumentException("ID de producto inválido.");
            return await _precioRepository.GetPrecioVigenteAsync(productoId, monedaId);
        }

        public async Task<decimal> GetPrecioFinalAsync(int productoId, int monedaId)
        {
            return await _precioRepository.GetPrecioFinalAsync(productoId, monedaId);
        }

        public async Task<IEnumerable<PrecioProducto>> GetHistorialAsync(int productoId)
        {
            return await _precioRepository.GetHistorialByProductoAsync(productoId);
        }
    }
}
