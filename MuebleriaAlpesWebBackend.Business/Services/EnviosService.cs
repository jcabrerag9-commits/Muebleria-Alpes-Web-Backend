using MuebleriaAlpesWebBackend.Domain.DTOs.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class EnviosService : IEnviosService
    {
        private readonly IEnviosRepository _enviosRepository;

        public EnviosService(IEnviosRepository enviosRepository)
        {
            _enviosRepository = enviosRepository;
        }

        public async Task<int> RegistrarEnvioAsync(RegistrarEnvioRequest request)
        {
            return await _enviosRepository.RegistrarEnvioAsync(request);
        }

        public async Task<bool> ActualizarEnvioAsync(ActualizarEnvioRequest request)
        {
            return await _enviosRepository.ActualizarEnvioAsync(request);
        }

        public async Task<bool> CambiarEstadoEnvioAsync(CambiarEstadoEnvioRequest request)
        {
            return await _enviosRepository.CambiarEstadoEnvioAsync(request);
        }

        public async Task<bool> ConfirmarEntregaEnvioAsync(ConfirmarEntregaEnvioRequest request)
        {
            return await _enviosRepository.ConfirmarEntregaEnvioAsync(request);
        }

        public async Task<bool> MarcarOrdenEnviadaAsync(int ordenVentaId)
        {
            return await _enviosRepository.MarcarOrdenEnviadaAsync(ordenVentaId);
        }

        public async Task<EnvioResponse?> ObtenerEnvioAsync(int envioId)
        {
            return await _enviosRepository.ObtenerEnvioAsync(envioId);
        }

        public async Task<List<EnvioResumenResponse>> ListarEnviosPorOrdenAsync(int ordenVentaId)
        {
            return await _enviosRepository.ListarEnviosPorOrdenAsync(ordenVentaId);
        }

        public async Task<List<EnvioResumenResponse>> ListarEnviosPorEstadoAsync(string estado)
        {
            return await _enviosRepository.ListarEnviosPorEstadoAsync(estado);
        }

        public async Task<EnvioResponse?> BuscarEnvioPorGuiaAsync(string numeroGuia)
        {
            return await _enviosRepository.BuscarEnvioPorGuiaAsync(numeroGuia);
        }
    }
}