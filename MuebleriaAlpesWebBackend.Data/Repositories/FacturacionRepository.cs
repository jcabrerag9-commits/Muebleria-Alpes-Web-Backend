using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class FacturacionRepository : IFacturacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public FacturacionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<FacturacionResponse<int?>> GenerarFacturaAsync(GenerarFacturaRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_orden_id", request.OrdenId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_pago_id", request.PagoId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_usuario_id", request.UsuarioId, DbType.Int32, ParameterDirection.Input);
            
            // Parámetros de Salida
            parameters.Add("p_factura_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_FACTURACION.SP_GENERAR_FACTURA_ORDEN", parameters, commandType: CommandType.StoredProcedure);

            return new FacturacionResponse<int?>
            {
                Resultado = parameters.Get<string>("p_resultado"),
                Mensaje = parameters.Get<string>("p_mensaje"),
                Data = parameters.Get<int?>("p_factura_id")
            };
        }

        public async Task<FacturacionResponse<bool>> AnularFacturaAsync(AnularFacturaRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_factura_id", request.FacturaId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_motivo", request.Motivo, DbType.String, ParameterDirection.Input);
            parameters.Add("p_usuario_id", request.UsuarioId, DbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_FACTURACION.SP_ANULAR_FACTURA", parameters, commandType: CommandType.StoredProcedure);

            var resultado = parameters.Get<string>("p_resultado");
            return new FacturacionResponse<bool>
            {
                Resultado = resultado,
                Mensaje = parameters.Get<string>("p_mensaje"),
                Data = resultado == "EXITO"
            };
        }

        public async Task<FacturaDTO?> ObtenerFacturaPorIdAsync(int facturaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = "PKG_FACTURACION.SP_OBTENER_FACTURA";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_factura_id", OracleDbType.Int32).Value = facturaId;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToFactura(reader);
            }

            return null;
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId)
        {
            var facturas = new List<FacturaDTO>();

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = "PKG_FACTURACION.SP_LISTAR_FACTURAS_CLIENTE";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_cliente_id", OracleDbType.Int32).Value = clienteId;
            command.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = null;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                facturas.Add(MapReaderToFactura(reader));
            }

            return facturas;
        }

        public async Task<bool> ActualizarEstadoFacturaAsync(int facturaId, string estado, IDbTransaction transaction = null)
        {
            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            string sql = "UPDATE ALP_FACTURA SET FAC_ESTADO = :estado WHERE FAC_FACTURA = :facturaId";
            
            var rows = await connection.ExecuteAsync(sql, new { estado, facturaId }, transaction: transaction);
            return rows > 0;
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerTodasAsync(string estado = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT FAC_FACTURA as FacturaId, VEN_ORDEN_VENTA as OrdenId, CLI_CLIENTE as ClienteId, 
                       FAC_NUMERO as Numero, FAC_SERIE as Serie, FAC_SUBTOTAL as Subtotal, 
                       FAC_IMPUESTOS as Impuestos, FAC_TOTAL as Total, FAC_ESTADO as Estado, 
                       FAC_FECHA_EMISION as FechaEmision 
                FROM ALP_FACTURA 
                WHERE (:estado IS NULL OR FAC_ESTADO = :estado)
                ORDER BY FAC_FECHA_EMISION DESC";
            
            return await connection.QueryAsync<FacturaDTO>(sql, new { estado });
        }

        public async Task<object?> ObtenerDetallePorIdAsync(int facturaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            
            using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = "PKG_FACTURACION.SP_OBTENER_FACTURA";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("p_factura_id", OracleDbType.Int32).Value = facturaId;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToFactura(reader);
            }
            return null;
        }

        private FacturaDTO MapReaderToFactura(IDataReader reader)
        {
            return new FacturaDTO
            {
                FacturaId = reader.GetInt32(reader.GetOrdinal("FAC_FACTURA")),
                OrdenId = reader.GetInt32(reader.GetOrdinal("VEN_ORDEN_VENTA")),
                ClienteId = reader.GetInt32(reader.GetOrdinal("CLI_CLIENTE")),
                Numero = reader.GetString(reader.GetOrdinal("FAC_NUMERO")),
                Serie = reader.IsDBNull(reader.GetOrdinal("FAC_SERIE")) ? string.Empty : reader.GetString(reader.GetOrdinal("FAC_SERIE")),
                Subtotal = reader.GetDecimal(reader.GetOrdinal("FAC_SUBTOTAL")),
                Impuestos = reader.GetDecimal(reader.GetOrdinal("FAC_IMPUESTOS")),
                Total = reader.GetDecimal(reader.GetOrdinal("FAC_TOTAL")),
                Estado = reader.GetString(reader.GetOrdinal("FAC_ESTADO")),
                FechaEmision = reader.GetDateTime(reader.GetOrdinal("FAC_FECHA_EMISION"))
            };
        }
    }
}
