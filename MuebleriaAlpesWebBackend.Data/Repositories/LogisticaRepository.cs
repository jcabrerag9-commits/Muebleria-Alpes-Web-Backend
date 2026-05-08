using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Logistica;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class LogisticaRepository : ILogisticaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public LogisticaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BaseResponse<CrearEnvioDataDto>> CrearEnvioAsync(CrearEnvioRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CREAR_ENVIO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            // --- Parámetros IN ---
            cmd.Parameters.Add("p_orden_id", OracleDbType.Int32, request.OrdenId, ParameterDirection.Input);
            cmd.Parameters.Add("p_direccion_id", OracleDbType.Int32, request.DireccionId, ParameterDirection.Input);
            cmd.Parameters.Add("p_transportista", OracleDbType.Varchar2, request.Transportista, ParameterDirection.Input);
            cmd.Parameters.Add("p_costo_envio", OracleDbType.Decimal, request.CostoEnvio, ParameterDirection.Input);

            // --- Parámetros OUT escalares ---
            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            var pEnvioId = new OracleParameter("p_envio_id", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(pEnvioId);

            await cmd.ExecuteNonQueryAsync();

            var resultadoStr = pResultado.Value?.ToString() ?? "ERROR";
            var mensajeStr = pMensaje.Value?.ToString() ?? "Error desconocido al crear el envío";
            
            var envioIdVal = 0;
            if (pEnvioId.Value is OracleDecimal idDec && !idDec.IsNull)
            {
                envioIdVal = idDec.ToInt32();
            }

            return new BaseResponse<CrearEnvioDataDto>
            {
                Resultado = resultadoStr,
                Mensaje = mensajeStr,
                Data = resultadoStr == "EXITO" ? new CrearEnvioDataDto { EnvioId = envioIdVal } : null
            };
        }

        public async Task<BaseResponse> ActualizarEstadoEnvioAsync(ActualizarEstadoEnvioRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_ACTUALIZAR_ESTADO_ENVIO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            // --- Parámetros IN ---
            cmd.Parameters.Add("p_envio_id", OracleDbType.Int32, request.EnvioId, ParameterDirection.Input);
            cmd.Parameters.Add("p_nuevo_estado", OracleDbType.Varchar2, request.NuevoEstado, ParameterDirection.Input);
            cmd.Parameters.Add("p_guia", OracleDbType.Varchar2, request.Guia ?? (object)DBNull.Value, ParameterDirection.Input);

            // --- Parámetros OUT escalares ---
            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? "ERROR",
                Mensaje = pMensaje.Value?.ToString() ?? "Error desconocido al actualizar el estado del envío"
            };
        }
    }
}
