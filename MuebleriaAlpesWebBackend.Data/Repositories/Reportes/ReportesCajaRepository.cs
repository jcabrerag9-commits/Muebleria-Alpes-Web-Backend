using System;
using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCaja;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Reportes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories.Reportes
{
    public class ReportesCajaRepository : IReportesCajaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ReportesCajaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TotalVentasCorteCajaResponse> TotalVentasCorteCajaAsync(CorteCajaBaseRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_CAJA.FN_TOTAL_VENTAS_CORTE_CAJA");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_rcc_reporte_corte_caja", OracleDbType.Int32).Value = request.CorteCajaId;

            await command.ExecuteNonQueryAsync();

            return new TotalVentasCorteCajaResponse
            {
                TotalVentas = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<DiferenciaCorteCajaResponse> DiferenciaCorteCajaAsync(CorteCajaBaseRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_CAJA.FN_DIFERENCIA_CORTE_CAJA");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_rcc_reporte_corte_caja", OracleDbType.Int32).Value = request.CorteCajaId;

            await command.ExecuteNonQueryAsync();

            return new DiferenciaCorteCajaResponse
            {
                DiferenciaCaja = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<List<ReporteCorteCajaItemResponse>> GenerarReporteCorteCajaAsync(GenerarReporteCorteCajaRequest request)
        {
            var resultado = new List<ReporteCorteCajaItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_CAJA.SP_GENERAR_REPORTE_CORTE_CAJA");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = ValorDb(request.Estado);
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteCorteCajaItemResponse
                {
                    CorteCajaId = ConvertirInt(reader["RCC_REPORTE_CORTE_CAJA"]),
                    FechaCorte = ConvertirDateTimeNullable(reader["RCC_FECHA_CORTE"]),
                    MontoInicial = ConvertirDecimal(reader["RCC_MONTO_INICIAL"]),
                    MontoFinal = ConvertirDecimal(reader["RCC_MONTO_FINAL"]),
                    TotalVentas = ConvertirDecimal(reader["RCC_TOTAL_VENTAS"]),
                    Observacion = ConvertirString(reader["RCC_OBSERVACION"]),
                    Estado = ConvertirString(reader["RCC_ESTADO"]),
                    DiferenciaCaja = ConvertirDecimal(reader["DIFERENCIA_CAJA"])
                });
            }

            return resultado;
        }


        public async Task<List<FiltroCorteCajaResponse>> ListarCortesCajaFiltroAsync(string? estado)
        {
            var resultado = new List<FiltroCorteCajaResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT RCC_REPORTE_CORTE_CAJA,
                       RCC_FECHA_CORTE,
                       RCC_MONTO_INICIAL,
                       RCC_MONTO_FINAL,
                       RCC_TOTAL_VENTAS,
                       RCC_ESTADO
                  FROM ALP_CORTE_CAJA
                 WHERE (:p_estado IS NULL OR RCC_ESTADO = :p_estado)
                 ORDER BY RCC_FECHA_CORTE DESC, RCC_REPORTE_CORTE_CAJA DESC";

            using var command = new OracleCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                BindByName = true
            };
            command.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = ValorDb(string.IsNullOrWhiteSpace(estado) ? null : estado);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new FiltroCorteCajaResponse
                {
                    CorteCajaId = ConvertirInt(reader["RCC_REPORTE_CORTE_CAJA"]),
                    FechaCorte = ConvertirDateTimeNullable(reader["RCC_FECHA_CORTE"]),
                    MontoInicial = ConvertirDecimal(reader["RCC_MONTO_INICIAL"]),
                    MontoFinal = ConvertirDecimal(reader["RCC_MONTO_FINAL"]),
                    TotalVentas = ConvertirDecimal(reader["RCC_TOTAL_VENTAS"]),
                    Estado = ConvertirString(reader["RCC_ESTADO"])
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

        private static decimal ConvertirDecimal(object? value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            if (value is OracleDecimal od)
            {
                return od.IsNull ? 0m : od.Value;
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