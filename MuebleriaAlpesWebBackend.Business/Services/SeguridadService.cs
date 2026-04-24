using MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class SeguridadService : ISeguridadService
    {
        private readonly ISeguridadRepository _seguridadRepository;

        public SeguridadService(ISeguridadRepository seguridadRepository)
        {
            _seguridadRepository = seguridadRepository;
        }

        public async Task<CrearUsuarioResponse> CrearUsuarioAsync(CrearUsuarioRequest request)
        {
            return await _seguridadRepository.CrearUsuarioAsync(request);
        }

        public async Task<bool> ActualizarUsuarioAsync(ActualizarUsuarioRequest request)
        {
            return await _seguridadRepository.ActualizarUsuarioAsync(request);
        }

        public async Task<bool> CambiarPasswordUsuarioAsync(CambiarPasswordUsuarioRequest request)
        {
            return await _seguridadRepository.CambiarPasswordUsuarioAsync(request);
        }

        public async Task<bool> BloquearUsuarioAsync(BloquearUsuarioRequest request)
        {
            return await _seguridadRepository.BloquearUsuarioAsync(request);
        }

        public async Task<bool> DesbloquearUsuarioAsync(DesbloquearUsuarioRequest request)
        {
            return await _seguridadRepository.DesbloquearUsuarioAsync(request);
        }

        public async Task<bool> EliminarUsuarioLogicoAsync(EliminarUsuarioLogicoRequest request)
        {
            return await _seguridadRepository.EliminarUsuarioLogicoAsync(request);
        }

        public async Task<CrearRolResponse> CrearRolAsync(CrearRolRequest request)
        {
            return await _seguridadRepository.CrearRolAsync(request);
        }

        public async Task<bool> ActualizarRolAsync(ActualizarRolRequest request)
        {
            return await _seguridadRepository.ActualizarRolAsync(request);
        }

        public async Task<bool> EliminarRolLogicoAsync(EliminarRolLogicoRequest request)
        {
            return await _seguridadRepository.EliminarRolLogicoAsync(request);
        }

        public async Task<CrearPermisoResponse> CrearPermisoAsync(CrearPermisoRequest request)
        {
            return await _seguridadRepository.CrearPermisoAsync(request);
        }

        public async Task<bool> ActualizarPermisoAsync(ActualizarPermisoRequest request)
        {
            return await _seguridadRepository.ActualizarPermisoAsync(request);
        }

        public async Task<bool> EliminarPermisoLogicoAsync(EliminarPermisoLogicoRequest request)
        {
            return await _seguridadRepository.EliminarPermisoLogicoAsync(request);
        }

        public async Task<AsignarRolUsuarioResponse> AsignarRolUsuarioAsync(AsignarRolUsuarioRequest request)
        {
            return await _seguridadRepository.AsignarRolUsuarioAsync(request);
        }

        public async Task<bool> QuitarRolUsuarioAsync(QuitarRolUsuarioRequest request)
        {
            return await _seguridadRepository.QuitarRolUsuarioAsync(request);
        }

        public async Task<AsignarPermisoRolResponse> AsignarPermisoRolAsync(AsignarPermisoRolRequest request)
        {
            return await _seguridadRepository.AsignarPermisoRolAsync(request);
        }

        public async Task<bool> QuitarPermisoRolAsync(QuitarPermisoRolRequest request)
        {
            return await _seguridadRepository.QuitarPermisoRolAsync(request);
        }

        public async Task<RegistrarBitacoraAccesoResponse> RegistrarBitacoraAccesoAsync(RegistrarBitacoraAccesoRequest request)
        {
            return await _seguridadRepository.RegistrarBitacoraAccesoAsync(request);
        }
    }
}