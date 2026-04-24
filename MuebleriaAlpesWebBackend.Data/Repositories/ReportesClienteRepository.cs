using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ReportesClienteRepository : IReportesClienteRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ReportesClienteRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TotalComprasClienteResponse> TotalComprasClienteAsync(ReporteClienteBaseRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_CLIENTE.FN_TOTAL_COMPRAS_CLIENTE");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = request.ClienteId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = ValorDb(request.FechaFin);

            await command.ExecuteNonQueryAsync();

            return new TotalComprasClienteResponse
            {
                TotalCompras = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<LtvClienteResponse> LtvClienteAsync(ReporteClienteBaseRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_CLIENTE.FN_LTV_CLIENTE");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = request.ClienteId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = ValorDb(request.FechaFin);

            await command.ExecuteNonQueryAsync();

            return new LtvClienteResponse
            {
                Ltv = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<TicketPromedioClienteResponse> TicketPromedioClienteAsync(ReporteClienteBaseRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_CLIENTE.FN_TICKET_PROMEDIO_CLIENTE");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = request.ClienteId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = ValorDb(request.FechaFin);

            await command.ExecuteNonQueryAsync();

            return new TicketPromedioClienteResponse
            {
                TicketPromedio = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<List<ReporteComprasClienteItemResponse>> GenerarReporteComprasPorClienteAsync(GenerarReporteComprasClienteRequest request)
        {
            var resultado = new List<ReporteComprasClienteItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_CLIENTE.SP_GENERAR_REPORTE_COMPRAS_POR_CLIENTE");

            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = request.ClienteId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = ValorDb(request.FechaFin);
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteComprasClienteItemResponse
                {
                    OrdenVentaId = ConvertirInt(reader["VEN_ORDEN_VENTA"]),
                    NumeroOrden = ConvertirString(reader["VEN_NUMERO_ORDEN"]),
                    FechaOrden = ConvertirDateTimeNullable(reader["VEN_FECHA_ORDEN"]),
                    ValorCompra = ConvertirDecimal(reader["VALOR_COMPRA"]),
                    FormaPago = ConvertirString(reader["FORMA_PAGO"]),
                    ProductoId = ConvertirIntNullable(reader["PRO_PRODUCTO"]),
                    Sku = ConvertirString(reader["PRO_SKU"]),
                    Mueble = ConvertirString(reader["MUEBLE"]),
                    Cantidad = ConvertirDecimal(reader["VDE_CANTIDAD"]),
                    PrecioUnitario = ConvertirDecimal(reader["VDE_PRECIO_UNITARIO"]),
                    TotalLinea = ConvertirDecimal(reader["TOTAL_LINEA"])
                });
            }

            return resultado;
        }

        private static OracleCommand CrearComandoProcedimiento(OracleConnection connection, string procedureName)
        {
            return new OracleCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };
        }

        private static OracleCommand CrearComandoFuncion(OracleConnection connection, string functionName)
        {
            return new OracleCommand(functionName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };
        }

        private static object ValorDb(object? value)
        {
            return value ?? DBNull.Value;
        }

        private static int ConvertirInt(object value)
        {
            return Convert.ToInt32(value);
        }

        private static int? ConvertirIntNullable(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var texto = value.ToString();
            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            return Convert.ToInt32(texto);
        }

        private static decimal ConvertirDecimal(object? value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            if (value is OracleDecimal oracleDecimal)
            {
                return oracleDecimal.IsNull ? 0m : oracleDecimal.Value;
            }

            var texto = value.ToString();

            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                return 0m;

            return decimal.Parse(texto);
        }

        private static DateTime? ConvertirDateTimeNullable(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            return Convert.ToDateTime(value);
        }

        private static string? ConvertirString(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var texto = value.ToString();
            return string.IsNullOrWhiteSpace(texto) ? null : texto;
        }
    }
}