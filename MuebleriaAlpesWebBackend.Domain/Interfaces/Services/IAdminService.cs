using MuebleriaAlpesWebBackend.Domain.DTOs.Admin;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IAdminService
    {
        Task<BaseResponse<AdminListarOrdenesDataDto>> ListarOrdenesAsync(ListarOrdenesFiltroDto filtro);
        Task<BaseResponse<AdminListarPagosDataDto>> ListarPagosAsync();
        Task<BaseResponse<AdminListarFacturasDataDto>> ListarFacturasAsync();
    }
}
