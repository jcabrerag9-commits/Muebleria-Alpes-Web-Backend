using MuebleriaAlpesWebBackend.Domain.DTOs.Admin;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<BaseResponse<AdminListarOrdenesDataDto>> ListarOrdenesAsync(ListarOrdenesFiltroDto filtro)
        {
            return await _adminRepository.ListarOrdenesAsync(filtro);
        }

        public async Task<BaseResponse<AdminListarPagosDataDto>> ListarPagosAsync()
        {
            return await _adminRepository.ListarPagosAsync();
        }

        public async Task<BaseResponse<AdminListarFacturasDataDto>> ListarFacturasAsync()
        {
            return await _adminRepository.ListarFacturasAsync();
        }
    }
}
