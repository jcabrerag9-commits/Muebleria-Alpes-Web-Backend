using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class FacturacionRepository : BaseRepository<FacturacionRepository>, IFacturacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public FacturacionRepository(OracleConnectionFactory connectionFactory, ILogger<FacturacionRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ApiResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(GenerarFacturaAsync), new { request.OrdenId, request.UsuarioId });

            var parameters = new OracleDynamicParameters();
            parameters.Add("p_orden_id", request.OrdenId);
            parameters.Add("p_pago_id", request.PagoId);
            parameters.Add("p_usuario_id", request.UsuarioId);
            
            parameters.Add("p_factura_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 4000);

            try
            {
                _logger.LogInformation("[FACTURA ORDEN] {OrdenId}", request.OrdenId);
                
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(new CommandDefinition("PKG_FACTURACION.SP_GENERAR_FACTURA_ORDEN", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));

                string resultado = parameters.Get<string>("p_resultado");
                _logger.LogInformation("[FACTURA RESULTADO] {Resultado}", resultado);

                var response = new ApiResponse<int?>
                {
                    Success = resultado == "EXITO",
                    Message = parameters.Get<string>("p_mensaje"),
                    Data = parameters.Get<int?>("p_factura_id")
                };

                LogOperacionExito(nameof(GenerarFacturaAsync), response.Resultado);
                return response;
            }
            catch (OracleException ex)
            {
                var mapped = MapearErrorOracle<int?>(ex);
                return new ApiResponse<int?> { Success = false, Message = mapped.Mensaje };
            }
        }

        public async Task<ApiResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(AnularFacturaAsync), new { request.FacturaId, request.UsuarioId });

            var parameters = new OracleDynamicParameters();
            parameters.Add("p_factura_id", request.FacturaId);
            parameters.Add("p_motivo", request.Motivo);
            parameters.Add("p_usuario_id", request.UsuarioId);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 4000);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(new CommandDefinition("PKG_FACTURACION.SP_ANULAR_FACTURA", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));

                string res = parameters.Get<string>("p_resultado");
                var response = new ApiResponse<bool>
                {
                    Success = res == "EXITO",
                    Message = parameters.Get<string>("p_mensaje"),
                    Data = res == "EXITO"
                };

                LogOperacionExito(nameof(AnularFacturaAsync), response.Resultado);
                return response;
            }
            catch (OracleException ex)
            {
                var mapped = MapearErrorOracle<bool>(ex, false);
                return new ApiResponse<bool> { Success = false, Message = mapped.Mensaje, Data = false };
            }
        }

        public async Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId, CancellationToken ct = default)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_factura_id", facturaId);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<FacturaDTO>(new CommandDefinition("PKG_FACTURACION.SP_OBTENER_FACTURA", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId, CancellationToken ct = default)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_cliente_id", clienteId);
            parameters.Add("p_estado", null);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<FacturaDTO>(new CommandDefinition("PKG_FACTURACION.SP_LISTAR_FACTURAS_CLIENTE", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerTodasAsync(string? estado = null, int? clienteId = null, string? nit = null, CancellationToken ct = default)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_estado", estado);
            parameters.Add("p_cliente_id", clienteId);
            parameters.Add("p_nit", nit);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<FacturaDTO>(new CommandDefinition("PKG_FACTURACION.SP_LISTAR_TODAS_FACTURAS", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        }

        public async Task<bool> ActualizarEstadoFacturaAsync(int facturaId, string estado, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            // Deprecado: Oracle controla el estado financieramente.
            _logger.LogWarning("Llamada a método deprecado: ActualizarEstadoFacturaAsync. FacturaId: {FacturaId}", facturaId);
            return await Task.FromResult(true);
        }

        public async Task<FacturaDTO?> ObtenerDetallePorIdAsync(int facturaId, CancellationToken ct = default)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_factura_id", facturaId);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            using var connection = _connectionFactory.CreateConnection();
            var factura = await connection.QueryFirstOrDefaultAsync<FacturaDTO>(new CommandDefinition("PKG_FACTURACION.SP_OBTENER_DETALLE_FACTURA", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            
            if (factura != null)
            {
                var itemsParams = new OracleDynamicParameters();
                itemsParams.Add("p_factura_id", facturaId);
                itemsParams.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                
                var items = await connection.QueryAsync<FacturaDetalleDTO>(new CommandDefinition("PKG_FACTURACION.SP_OBTENER_ITEMS_FACTURA", itemsParams, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                factura.Detalles = items.AsList();
            }

            _logger.LogInformation("[FACTURA DETALLE ENRIQUECIDO] ID={FacturaId} Items={Count}", facturaId, factura?.Detalles?.Count ?? 0);
            return factura;
        }

        public async Task<IEnumerable<OrdenPendienteDTO>> ObtenerOrdenesPendientesAsync(CancellationToken ct = default)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            using var connection = _connectionFactory.CreateConnection();
            var data = await connection.QueryAsync<OrdenPendienteDTO>(new CommandDefinition("PKG_FACTURACION.SP_LISTAR_ORDENES_FACTURABLES", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            
            return data;
        }
    }
}
