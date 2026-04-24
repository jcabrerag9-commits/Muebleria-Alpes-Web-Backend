using System;
using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.ReportesMarketing;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ReportesMarketingRepository : IReportesMarketingRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ReportesMarketingRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LtvMarketingResponse> LtvClienteAsync(ReporteMarketingClienteRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_MARKETING.FN_LTV_CLIENTE");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = request.ClienteId;
            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;

            await command.ExecuteNonQueryAsync();

            return new LtvMarketingResponse
            {
                Ltv = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<TasaRetencionResponse> TasaRetencionAsync(ReporteMarketingRangoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_MARKETING.FN_TASA_RETENCION");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;

            await command.ExecuteNonQueryAsync();

            return new TasaRetencionResponse
            {
                TasaRetencion = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<TasaConversionResponse> TasaConversionAsync(ReporteMarketingRangoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_REPORTES_MARKETING.FN_TASA_CONVERSION");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;

            await command.ExecuteNonQueryAsync();

            return new TasaConversionResponse
            {
                TasaConversion = ConvertirDecimal(returnParam.Value)
            };
        }

        public async Task<List<ReporteLtvItemResponse>> GenerarReporteLtvAsync(GenerarReporteMarketingRequest request)
        {
            var resultado = new List<ReporteLtvItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_MARKETING.SP_GENERAR_REPORTE_LTV");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var primerNombre = ConvertirString(reader["CLI_PRIMER_NOMBRE"]);
                var primerApellido = ConvertirString(reader["CLI_PRIMER_APELLIDO"]);

                resultado.Add(new ReporteLtvItemResponse
                {
                    ClienteId = ConvertirInt(reader["CLI_CLIENTE"]),
                    ClienteCodigo = ConvertirString(reader["CLI_CODIGO"]),
                    ClienteNombre = $"{primerNombre} {primerApellido}".Trim(),
                    Ltv = ConvertirDecimal(reader["LTV_TOTAL"])
                });
            }

            return resultado;
        }

        public async Task<List<ReporteActividadItemResponse>> GenerarReporteActividadAsync(GenerarReporteMarketingRequest request)
        {
            var resultado = new List<ReporteActividadItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_MARKETING.SP_GENERAR_REPORTE_ACTIVIDAD");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteActividadItemResponse
                {
                    TipoEvento = ConvertirString(reader["EVA_TIPO_EVENTO"]),
                    TotalEventos = ConvertirInt(reader["TOTAL_EVENTOS"])
                });
            }

            return resultado;
        }

        public async Task<List<ReporteRetencionItemResponse>> GenerarReporteRetencionAsync(GenerarReporteMarketingRequest request)
        {
            var resultado = new List<ReporteRetencionItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_MARKETING.SP_GENERAR_REPORTE_RETENCION");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteRetencionItemResponse
                {
                    Periodo = $"{request.FechaInicio:yyyy-MM-dd} a {request.FechaFin:yyyy-MM-dd}",
                    ClientesBase = ConvertirInt(reader["CLIENTES_NUEVOS"]),
                    ClientesRetenidos = ConvertirInt(reader["CLIENTES_RETENIDOS"]),
                    TasaRetencion = ConvertirDecimal(reader["TASA_RETENCION"])
                });
            }

            return resultado;
        }

        public async Task<List<ReporteCohorteItemResponse>> GenerarReporteCohorteAsync(GenerarReporteMarketingRequest request)
        {
            var resultado = new List<ReporteCohorteItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_MARKETING.SP_GENERAR_REPORTE_COHORTE");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var totalClientes = ConvertirInt(reader["TOTAL_CLIENTES"]);
                var clientesConCompra = ConvertirInt(reader["CLIENTES_CON_COMPRA"]);
                var tasa = totalClientes == 0 ? 0m : Math.Round((decimal)clientesConCompra / totalClientes * 100m, 2);

                resultado.Add(new ReporteCohorteItemResponse
                {
                    Cohorte = ConvertirDateTimeNullable(reader["FECHA_COHORTE"])?.ToString("yyyy-MM-dd"),
                    TotalClientes = totalClientes,
                    TotalCompradores = clientesConCompra,
                    TasaConversion = tasa
                });
            }

            return resultado;
        }

        public async Task<List<ReporteRemarketingItemResponse>> GenerarReporteRemarketingAsync(GenerarReporteRemarketingRequest request)
        {
            var resultado = new List<ReporteRemarketingItemResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_REPORTES_MARKETING.SP_GENERAR_REPORTE_REMARKETING");

            command.Parameters.Add("p_fecha_inicio", OracleDbType.Date).Value = request.FechaInicio;
            command.Parameters.Add("p_fecha_fin", OracleDbType.Date).Value = request.FechaFin;
            command.Parameters.Add("p_dias_inactividad", OracleDbType.Int32).Value = request.DiasInactividad;
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = ValorDb(request.UsuarioId);
            command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new ReporteRemarketingItemResponse
                {
                    Metrica = ConvertirString(reader["METRICA"]),
                    Valor = ConvertirDecimal(reader["VALOR"])
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