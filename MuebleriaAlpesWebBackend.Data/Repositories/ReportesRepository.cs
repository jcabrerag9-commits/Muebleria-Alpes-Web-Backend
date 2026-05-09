using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ReportesRepository : IReportesRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ReportesRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BaseResponse> RegistrarEjecucionReporteAsync(RegistrarEjecucionReporteRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_REGISTRAR_EJECUCION_REPORTE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            // --- Parámetros IN ---
            cmd.Parameters.Add("p_usuario_id", OracleDbType.Int32, request.UsuarioId, ParameterDirection.Input);
            cmd.Parameters.Add("p_nombre_reporte", OracleDbType.Varchar2, request.NombreReporte, ParameterDirection.Input);
            cmd.Parameters.Add("p_parametros", OracleDbType.Clob, request.Parametros, ParameterDirection.Input);
            cmd.Parameters.Add("p_tiempo_ms", OracleDbType.Int32, request.TiempoMs, ParameterDirection.Input);
            cmd.Parameters.Add("p_estado", OracleDbType.Varchar2, request.Estado, ParameterDirection.Input);

            // --- Parámetro OUT escalar ---
            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            await cmd.ExecuteNonQueryAsync();

            var resultadoStr = pResultado.Value?.ToString() ?? "ERROR";

            return new BaseResponse
            {
                Resultado = resultadoStr,
                Mensaje = resultadoStr == "EXITO" ? "Reporte registrado correctamente." : "Error al registrar la ejecución del reporte."
            };
        }
    }
}
