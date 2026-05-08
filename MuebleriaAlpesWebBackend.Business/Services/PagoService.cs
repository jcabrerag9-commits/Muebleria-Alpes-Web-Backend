using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IFacturacionRepository _facturacionRepository;
        private readonly MuebleriaAlpesWebBackend.Data.Connection.OracleConnectionFactory _connectionFactory;

        public PagoService(IPagoRepository pagoRepository, IFacturacionRepository facturacionRepository, MuebleriaAlpesWebBackend.Data.Connection.OracleConnectionFactory connectionFactory)
        {
            _pagoRepository = pagoRepository;
            _facturacionRepository = facturacionRepository;
            _connectionFactory = connectionFactory;
        }

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                if (connection.State != System.Data.ConnectionState.Open) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var response = await _pagoRepository.ProcesarPagoAsync(request, transaction);

                        if (response.IsSuccess && response.FacturaId.HasValue)
                        {
                            decimal saldo = await _pagoRepository.ObtenerSaldoPendienteAsync(response.FacturaId.Value, transaction);

                            if (saldo <= 0)
                            {
                                await _facturacionRepository.ActualizarEstadoFacturaAsync(response.FacturaId.Value, "PAGADA", transaction);
                            }
                        }
                        else if (!response.IsSuccess)
                        {
                            transaction.Rollback();
                            return response;
                        }

                        transaction.Commit();
                        return response;
                    }
                    catch (System.Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<object?> ObtenerPorIdAsync(int id)
        {
            return await _pagoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<object>> ObtenerTodosAsync()
        {
            return await _pagoRepository.ObtenerTodosAsync();
        }

        public async Task<IEnumerable<object>> ObtenerPorFacturaIdAsync(int facturaId)
        {
            return await _pagoRepository.ObtenerPorFacturaIdAsync(facturaId);
        }

        public async Task<bool> AnularPagoAsync(int id)
        {
            return await _pagoRepository.AnularPagoAsync(id);
        }
    }
}
