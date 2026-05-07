using MuebleriaAlpesWebBackend.Domain.DTOs.Carrito;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ICarritoRepository
    {
        Task<BaseResponse<AgregarProductoCarritoDataDto>> AgregarProductoAsync(AgregarProductoCarritoRequestDto request);
        Task<BaseResponse> ActualizarCantidadAsync(ActualizarCantidadCarritoRequestDto request);
        Task<BaseResponse> EliminarProductoAsync(int detalleId);
        Task<BaseResponse> VaciarCarritoAsync(int carritoId);
        Task<BaseResponse<CalcularTotalCarritoDataDto>> CalcularTotalAsync(int carritoId);
        Task<BaseResponse<ConvertirOrdenCarritoDataDto>> ConvertirOrdenAsync(ConvertirOrdenCarritoRequestDto request);
    }
}
