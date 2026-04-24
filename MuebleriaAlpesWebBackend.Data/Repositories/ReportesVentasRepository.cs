using System;
using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesVentas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ReportesVentasRepository : IReportesVentasRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ReportesVentasRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TotalVentasRangoResponse> TotalVentasRangoAsync(ReporteVentasRangoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_VENTAS.FN_TOTAL_VENTAS_RANGO");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;

            await command.ExecuteNonQueryAsync();

            return new TotalVentasRangoResponse
            {
                TotalVentas = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<TotalVentasCiudadResponse> TotalVentasPorCiudadAsync(ReporteVentasCiudadRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_VENTAS.FN_TOTAL_VENTAS_POR_CIUDAD");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_ciu_ciudad", OracleDbType.Int32).Value = request.CiudadId;

            await command.ExecuteNonQueryAsync();

            return new TotalVentasCiudadResponse
            {
                TotalVentas = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<ProductoMasVendidoResponse> ProductoMasVendidoRangoAsync(ReporteProductoMasVendidoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_VENTAS.FN_PRODUCTO_MAS_VENDIDO_RANGO");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_ciu_ciudad", OracleDbType.Int32).Value = ValorDb(request.CiudadId);

            await command.ExecuteNonQueryAsync();

            return new ProductoMasVendidoResponse
            {
                ProductoId = ConvertirIntNullable(returnParam.Value)
            };
        }

        public async Task<TotalIngresosCanalResponse> TotalIngresosPorCanalAsync(ReporteIngresosCanalRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_VENTAS.FN_TOTAL_INGRESOS_POR_CANAL");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_cve_canal_venta", OracleDbType.Int32).Value = request.CanalVentaId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = ValorDb(request.FechaFin);

            await command.ExecuteNonQueryAsync();

            return new TotalIngresosCanalResponse
            {
                TotalIngresos = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<List<ReporteVentasDiariasItemResponse>> GenerarReporteVentasDiariasAsync(GenerarReporteVentasDiariasRequest request)
        {
            var resultado = new List<ReporteVentasDiariasItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_VENTAS.SP_GENERAR_REPORTE_VENTAS_DIARIAS");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_ciu_ciudad", OracleDbType.Int32).Value = ValorDb(request.CiudadId);
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteVentasDiariasItemResponse
                {
                    FechaVenta = ConvertirDateTimeNullable(reader["FECHA_VENTA"]),
                    TotalOrdenes = ConvertirInt(reader["TOTAL_ORDENES"]),
                    TotalUnidades = ConvertirDecimal(reader["TOTAL_UNIDADES"]),
                    TotalVendido = ConvertirDecimal(reader["TOTAL_VENDIDO"])
                });
            }

            return resultado;
        }

        public async Task<List<ReporteProductoMasVendidoItemResponse>> GenerarReporteProductoMasVendidoAsync(ReporteProductoMasVendidoRequest request)
        {
            var resultado = new List<ReporteProductoMasVendidoItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_VENTAS.SP_GENERAR_REPORTE_PRODUCTO_MAS_VENDIDO");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_ciu_ciudad", OracleDbType.Int32).Value = ValorDb(request.CiudadId);
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteProductoMasVendidoItemResponse
                {
                    ProductoId = ConvertirIntNullable(reader["PRO_PRODUCTO"]),
                    Sku = ConvertirString(reader["PRO_SKU"]),
                    Producto = ConvertirString(reader["PRO_NOMBRE"]),
                    TipoMueble = ConvertirString(reader["TIPO_MUEBLE"]),
                    TotalUnidades = ConvertirDecimal(reader["TOTAL_UNIDADES"]),
                    TotalVendido = ConvertirDecimal(reader["TOTAL_VENDIDO"])
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
            if (value is OracleDecimal od)
            {
                return od.IsNull ? 0 : od.ToInt32();
            }

            return Convert.ToInt32(value);
        }

        private static int? ConvertirIntNullable(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (value is OracleDecimal od)
            {
                return od.IsNull ? null : od.ToInt32();
            }

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