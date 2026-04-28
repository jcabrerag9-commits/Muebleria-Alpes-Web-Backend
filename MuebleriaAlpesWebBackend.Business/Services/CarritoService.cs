using MuebleriaAlpesWebBackend.Domain.DTOs.Carrito;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;

        public CarritoService(ICarritoRepository carritoRepository)
        {
            _carritoRepository = carritoRepository;
        }

        public async Task<BaseResponse<AgregarProductoCarritoDataDto>> AgregarProductoAsync(AgregarProductoCarritoRequestDto request)
        {
            if (request.Cantidad <= 0)
            {
                return new BaseResponse<AgregarProductoCarritoDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "La cantidad debe ser mayor a cero"
                };
            }

            return await _carritoRepository.AgregarProductoAsync(request);
        }

        public async Task<BaseResponse> ActualizarCantidadAsync(ActualizarCantidadCarritoRequestDto request)
        {
            if (request.NuevaCantidad <= 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "La nueva cantidad debe ser mayor a cero"
                };
            }

            return await _carritoRepository.ActualizarCantidadAsync(request);
        }

        public async Task<BaseResponse> EliminarProductoAsync(int detalleId)
        {
            return await _carritoRepository.EliminarProductoAsync(detalleId);
        }

        public async Task<BaseResponse> VaciarCarritoAsync(int carritoId)
        {
            return await _carritoRepository.VaciarCarritoAsync(carritoId);
        }

        public async Task<BaseResponse<CalcularTotalCarritoDataDto>> CalcularTotalAsync(int carritoId)
        {
            return await _carritoRepository.CalcularTotalAsync(carritoId);
        }

        public async Task<BaseResponse<ConvertirOrdenCarritoDataDto>> ConvertirOrdenAsync(ConvertirOrdenCarritoRequestDto request)
        {
            return await _carritoRepository.ConvertirOrdenAsync(request);
        }
    }
}
