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
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "PKG_FACTURACION.SP_OBTENER_FACTURA";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_factura_id", OracleDbType.Int32).Value = facturaId;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToFactura(reader);
            }

            return null;
        }

        public async Task<IEnumerable<FacturaDTO>> ObtenerFacturasPorClienteAsync(int clienteId)
        {
            var facturas = new List<FacturaDTO>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "PKG_FACTURACION.SP_LISTAR_FACTURAS_CLIENTE";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_cliente_id", OracleDbType.Int32).Value = clienteId;
            command.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = null;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                facturas.Add(MapReaderToFactura(reader));
            }

            return facturas;
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
