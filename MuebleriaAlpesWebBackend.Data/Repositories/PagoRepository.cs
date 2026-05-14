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
    public class PagoRepository : BaseRepository<PagoRepository>, IPagoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PagoRepository(OracleConnectionFactory connectionFactory, ILogger<PagoRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, IDbTransaction transaction = null, System.Threading.CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(ProcesarPagoAsync), new { request.OrdenId, request.FacturaId, request.Monto, request.UsuarioId });

            using var internalConnection = transaction == null ? _connectionFactory.CreateConnection() : null;
            var connection = transaction?.Connection ?? internalConnection;

            // Resolve OrdenId if only FacturaId is provided
            if ((!request.OrdenId.HasValue || request.OrdenId <= 0) && request.FacturaId.HasValue)
            {
                var ordenId = await connection!.QueryFirstOrDefaultAsync<int?>(
                    "SELECT VEN_ORDEN_VENTA FROM ALP_FACTURA WHERE FAC_FACTURA = :FacturaId",
                    new { FacturaId = request.FacturaId.Value },
                    transaction: transaction
                );

                if (ordenId.HasValue)
                {
                    request.OrdenId = ordenId.Value;
                }
                else
                {
                    return new ProcesarPagoResponse { Resultado = "ERROR", Mensaje = "La factura especificada no existe." };
                }
            }

            if (!request.OrdenId.HasValue || request.OrdenId <= 0)
            {
                return new ProcesarPagoResponse { Resultado = "ERROR", Mensaje = "Se requiere el ID de la orden o factura." };
            }

            var parameters = new OracleDynamicParameters();
            parameters.Add("p_orden_id", request.OrdenId.Value);
            parameters.Add("p_forma_pago", request.FormaPagoId);
            parameters.Add("p_monto", request.Monto);
            parameters.Add("p_moneda_id", request.MonedaId == 0 ? 1 : request.MonedaId);
            parameters.Add("p_referencia", request.Referencia);
            parameters.Add("p_usuario_id", request.UsuarioId ?? 999);
            
            parameters.Add("p_pago_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_factura_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 4000);

            _logger.LogInformation("[PAGO ORDEN] {OrdenId}", request.OrdenId);
            _logger.LogInformation("[PAGO FACTURA] {FacturaId}", request.FacturaId);

            try
            {
                await connection!.ExecuteAsync(new CommandDefinition(
                    "SP_PROCESAR_PAGO", 
                    parameters, 
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));

                string res = parameters.Get<string>("p_resultado");
                string msg = parameters.Get<string>("p_mensaje");
                bool isSuccess = res == "EXITO" || res == "OK";

                var response = new ProcesarPagoResponse
                {
                    Data = isSuccess ? new ProcesarPagoData
                    {
                        PagoId = parameters.Get<int?>("p_pago_id"),
                        FacturaId = parameters.Get<int?>("p_factura_id")
                    } : null,
                    Success = isSuccess,
                    Resultado = res,
                    Message = msg
                };

                _logger.LogInformation("[PAGO] Factura={FacturaId} Monto={Monto}", response.FacturaId, request.Monto);
                
                _logger.LogInformation("[PAGO RESULTADO] {Resultado}", res);
                _logger.LogInformation("[PAGO MENSAJE] {Mensaje}", msg);

                LogOperacionExito(nameof(ProcesarPagoAsync), new { response.Success, response.PagoId });
                return response;
            }
            catch (OracleException ex)
            {
                var mapped = MapearErrorOracle<ProcesarPagoResponse>(ex);
                return new ProcesarPagoResponse { Success = false, Resultado = mapped.Resultado, Mensaje = mapped.Mensaje };
            }
        }

        public async Task<decimal> ObtenerSaldoPendienteOracleAsync(int facturaId, IDbTransaction transaction = null, System.Threading.CancellationToken ct = default)
        {
            using var internalConnection = transaction == null ? _connectionFactory.CreateConnection() : null;
            var connection = transaction?.Connection ?? internalConnection;

            string sql = "SELECT FN_SALDO_PENDIENTE_FACTURA(:facturaId) FROM DUAL";
            
            return await connection!.ExecuteScalarAsync<decimal>(new CommandDefinition(sql, new { facturaId }, transaction: transaction, cancellationToken: ct));
        }

        public async Task<PagoDTO?> ObtenerPorIdAsync(int id, System.Threading.CancellationToken ct = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_pago_id", id);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            var data = await connection.QueryFirstOrDefaultAsync<PagoDTO>(new CommandDefinition("SP_OBTENER_PAGO", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            _logger.LogInformation("[PAGO RAW DB] ID={Id} Data={@Data}", id, data);
            return data;
        }

        public async Task<IEnumerable<PagoDTO>> ObtenerTodosAsync(System.Threading.CancellationToken ct = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            var data = await connection.QueryAsync<PagoDTO>(new CommandDefinition("SP_LISTAR_PAGOS", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            _logger.LogInformation("[PAGOS LISTADO RAW DB] Count={Count}", data.AsList().Count);
            return data;
        }

        public async Task<IEnumerable<PagoDTO>> ObtenerPorFacturaIdAsync(int facturaId, System.Threading.CancellationToken ct = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_factura_id", facturaId);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            var data = await connection.QueryAsync<PagoDTO>(new CommandDefinition("SP_LISTAR_PAGOS_FACTURA", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            _logger.LogInformation("[PAGOS FACTURA RAW DB] FacturaId={FacturaId} Count={Count}", facturaId, data.AsList().Count);
            return data;
        }

        public async Task<ApiResponse<bool>> AnularPagoAsync(int id, string motivo, int usuarioId, System.Threading.CancellationToken ct = default)
        {
            _logger.LogInformation("[PAGO REPOSITORY] Anulando Pago Id={Id} Motivo={Motivo}", id, motivo);
            LogOperacionInicio(nameof(AnularPagoAsync), new { id, usuarioId });

            var parameters = new OracleDynamicParameters();
            parameters.Add("p_pago_id", id);
            parameters.Add("p_motivo", motivo);
            parameters.Add("p_usuario_id", usuarioId);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 4000);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(new CommandDefinition("SP_ANULAR_PAGO", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                string res = parameters.Get<string>("p_resultado");
                string msg = parameters.Get<string>("p_mensaje");
                
                LogOperacionExito(nameof(AnularPagoAsync), res);
                
                return new ApiResponse<bool>
                {
                    Success = res == "EXITO",
                    Resultado = res,
                    Message = msg,
                    Data = res == "EXITO"
                };
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "[ANULAR PAGO ORACLE ERROR] PagoId={Id}", id);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Resultado = "ERROR_ORACLE",
                    Message = $"Error de base de datos: {ex.Message} (ORA-{ex.Number})",
                    Data = false
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "[ANULAR PAGO GENERIC ERROR] PagoId={Id}", id);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Resultado = "ERROR_SISTEMA",
                    Message = ex.Message,
                    Data = false
                };
            }
        }

        public async Task<IEnumerable<OrdenPendientePagoDTO>> ObtenerOrdenesPendientesAsync(System.Threading.CancellationToken ct = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            var data = await connection.QueryAsync<OrdenPendientePagoDTO>(new CommandDefinition("PKG_FACTURACION.SP_OBTENER_ORDENES_PENDIENTES_PAGO", parameters, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            
            var list = data.AsList();
            foreach (var item in list)
            {
                _logger.LogInformation("[SALDO ERP] Orden={OrdenId} Total={Total} Pagado={Pagado} Pendiente={Pendiente}", 
                    item.OrdenId, item.TotalFactura, item.TotalPagado, item.SaldoPendiente);
            }

            _logger.LogInformation("[ORDENES PENDIENTES] {Count}", list.Count);
            return list;
        }
    }
}

