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

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request)
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

            using var connection = _connectionFactory.CreateConnection();
            
            await connection.ExecuteAsync(
                "SP_PROCESAR_PAGO", 
                parameters, 
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
    }
}
