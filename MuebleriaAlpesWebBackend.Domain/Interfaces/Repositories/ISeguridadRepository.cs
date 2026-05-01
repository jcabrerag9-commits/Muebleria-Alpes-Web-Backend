using MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface ISeguridadRepository
    {
        Task<List<UsuarioConsultaResponse>> ListarUsuariosAsync(string? estado = null);
        Task<UsuarioConsultaResponse?> ObtenerUsuarioAsync(int usuarioId);

        Task<CrearUsuarioResponse> CrearUsuarioAsync(CrearUsuarioRequest request);
        Task<bool> ActualizarUsuarioAsync(ActualizarUsuarioRequest request);
        Task<bool> CambiarPasswordUsuarioAsync(CambiarPasswordUsuarioRequest request);
        Task<bool> BloquearUsuarioAsync(BloquearUsuarioRequest request);
        Task<bool> DesbloquearUsuarioAsync(DesbloquearUsuarioRequest request);
        Task<bool> EliminarUsuarioLogicoAsync(EliminarUsuarioLogicoRequest request);

        Task<List<RolConsultaResponse>> ListarRolesAsync(string? estado = null);
        Task<RolConsultaResponse?> ObtenerRolAsync(int rolId);

        Task<CrearRolResponse> CrearRolAsync(CrearRolRequest request);
        Task<bool> ActualizarRolAsync(ActualizarRolRequest request);
        Task<bool> EliminarRolLogicoAsync(EliminarRolLogicoRequest request);

        Task<List<PermisoConsultaResponse>> ListarPermisosAsync(string? estado = null);
        Task<PermisoConsultaResponse?> ObtenerPermisoAsync(int permisoId);

        Task<CrearPermisoResponse> CrearPermisoAsync(CrearPermisoRequest request);
        Task<bool> ActualizarPermisoAsync(ActualizarPermisoRequest request);
        Task<bool> EliminarPermisoLogicoAsync(EliminarPermisoLogicoRequest request);

        Task<AsignarRolUsuarioResponse> AsignarRolUsuarioAsync(AsignarRolUsuarioRequest request);
        Task<bool> QuitarRolUsuarioAsync(QuitarRolUsuarioRequest request);

        Task<AsignarPermisoRolResponse> AsignarPermisoRolAsync(AsignarPermisoRolRequest request);
        Task<bool> QuitarPermisoRolAsync(QuitarPermisoRolRequest request);

        Task<RegistrarBitacoraAccesoResponse> RegistrarBitacoraAccesoAsync(RegistrarBitacoraAccesoRequest request);
    }
}
