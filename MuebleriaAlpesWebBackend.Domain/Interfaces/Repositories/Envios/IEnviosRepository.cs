using MuebleriaAlpesWebBackend.Domain.DTOs.Envios;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Envios
{
    public interface IEnviosRepository
    {
        Task<int> RegistrarEnvioAsync(RegistrarEnvioRequest request);

        Task<bool> ActualizarEnvioAsync(ActualizarEnvioRequest request);

        Task<bool> CambiarEstadoEnvioAsync(CambiarEstadoEnvioRequest request);

        Task<bool> ConfirmarEntregaEnvioAsync(ConfirmarEntregaEnvioRequest request);

        Task<bool> MarcarOrdenEnviadaAsync(int ordenVentaId);

        Task<EnvioResponse?> ObtenerEnvioAsync(int envioId);

        Task<List<EnvioResumenResponse>> ListarEnviosPorOrdenAsync(int ordenVentaId);

        Task<List<EnvioResumenResponse>> ListarEnviosPorEstadoAsync(string estado);

        Task<EnvioResponse?> BuscarEnvioPorGuiaAsync(string numeroGuia);


        Task<List<FiltroOrdenDisponibleResponse>> ListarOrdenesDisponiblesFiltroAsync();

        Task<List<FiltroDireccionClienteResponse>> ListarDireccionesClienteFiltroAsync(int clienteId);
    }
}