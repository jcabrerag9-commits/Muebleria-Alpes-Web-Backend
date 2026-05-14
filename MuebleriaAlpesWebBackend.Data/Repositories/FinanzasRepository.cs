using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class FinanzasRepository : BaseRepository<FinanzasRepository>, IFinanzasRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public FinanzasRepository(OracleConnectionFactory connectionFactory, ILogger<FinanzasRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<HistorialFinancieroDTO>> ObtenerHistorialFinancieroAsync(HistorialFiltroRequest request, System.Threading.CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(ObtenerHistorialFinancieroAsync), request);

            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            
            parameters.Add("p_fecha_inicio", request.FechaInicio);
            parameters.Add("p_fecha_fin", request.FechaFin);
            parameters.Add("p_tipo_movimiento", request.TipoMovimiento);
            parameters.Add("p_factura_id", request.FacturaId);
            parameters.Add("p_cliente_id", request.ClienteId);
            parameters.Add("p_usuario_id", request.UsuarioId);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            try
            {
                var result = await connection.QueryAsync<HistorialFinancieroDTO>(new CommandDefinition(
                    "SP_OBTENER_HISTORIAL_FINANCIERO", 
                    parameters, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                LogOperacionExito(nameof(ObtenerHistorialFinancieroAsync));
                return result;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Error al obtener historial financiero.");
                throw;
            }
        }
    }
}

