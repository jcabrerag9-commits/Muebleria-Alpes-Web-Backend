using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PagoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, IDbTransaction transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_orden_id", request.OrdenId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_forma_pago", request.FormaPagoId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_monto", request.Monto, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_moneda_id", request.MonedaId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_referencia", request.Referencia, DbType.String, ParameterDirection.Input);
            
            // Parámetros de Salida (OUT)
            parameters.Add("p_pago_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_factura_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            
            await connection.ExecuteAsync(
                "SP_PROCESAR_PAGO", 
                parameters, 
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );

            return new ProcesarPagoResponse
            {
                PagoId = parameters.Get<int?>("p_pago_id"),
                FacturaId = parameters.Get<int?>("p_factura_id"),
                Resultado = parameters.Get<string>("p_resultado"),
                Mensaje = parameters.Get<string>("p_mensaje")
            };
        }

        public async Task<decimal> ObtenerSaldoPendienteAsync(int facturaId, IDbTransaction transaction = null)
        {
            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            string sql = @"
                SELECT (FAC_TOTAL - NVL((SELECT SUM(PAG_MONTO) 
                                        FROM ALP_PAGO 
                                        WHERE VEN_ORDEN_VENTA = F.VEN_ORDEN_VENTA), 0))
                FROM ALP_FACTURA F 
                WHERE FAC_FACTURA = :facturaId";
            
            return await connection.ExecuteScalarAsync<decimal>(sql, new { facturaId }, transaction: transaction);
        }

        public async Task<object?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync("SELECT * FROM ALP_PAGO WHERE PAG_PAGO = :id", new { id });
        }

        public async Task<IEnumerable<object>> ObtenerTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync("SELECT * FROM ALP_PAGO ORDER BY PAG_PAGO DESC");
        }

        public async Task<IEnumerable<object>> ObtenerPorFacturaIdAsync(int facturaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync(@"
                SELECT P.* 
                FROM ALP_PAGO P
                JOIN ALP_FACTURA F ON F.VEN_ORDEN_VENTA = P.VEN_ORDEN_VENTA
                WHERE F.FAC_FACTURA = :facturaId", new { facturaId });
        }

        public async Task<bool> AnularPagoAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.ExecuteAsync("UPDATE ALP_PAGO SET ESP_ESTADO_PAGO = (SELECT ESP_ESTADO_PAGO FROM ALP_ESTADO_PAGO WHERE ESP_CODIGO = 'ANULADO') WHERE PAG_PAGO = :id", new { id });
            return rows > 0;
        }
    }
}
