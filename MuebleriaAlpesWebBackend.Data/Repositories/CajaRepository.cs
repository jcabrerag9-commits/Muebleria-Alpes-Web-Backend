using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Caja;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public CajaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BaseResponse<AbrirCajaDataDto>> AbrirCajaAsync(AbrirCajaRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_ABRIR_CAJA";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_monto_inicial", OracleDbType.Decimal, request.MontoInicial, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            var pCorteId = new OracleParameter("p_corte_id", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add(pCorteId);

            await cmd.ExecuteNonQueryAsync();

            var resultadoStr = pResultado.Value?.ToString() ?? "ERROR";
            var mensajeStr = pMensaje.Value?.ToString() ?? "Error desconocido al abrir la caja";
            
            var corteIdVal = 0;
            if (pCorteId.Value is OracleDecimal idDec && !idDec.IsNull)
            {
                corteIdVal = idDec.ToInt32();
            }

            return new BaseResponse<AbrirCajaDataDto>
            {
                Resultado = resultadoStr,
                Mensaje = mensajeStr,
                Data = resultadoStr == "EXITO" ? new AbrirCajaDataDto { CorteId = corteIdVal } : null
            };
        }

        public async Task<BaseResponse> CerrarCajaAsync(CerrarCajaRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CERRAR_CAJA";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_corte_id", OracleDbType.Int32, request.CorteId, ParameterDirection.Input);
            cmd.Parameters.Add("p_monto_final", OracleDbType.Decimal, request.MontoFinal, ParameterDirection.Input);
            cmd.Parameters.Add("p_observacion", OracleDbType.Varchar2, request.Observacion, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? "ERROR",
                Mensaje = pMensaje.Value?.ToString() ?? "Error desconocido al cerrar la caja"
            };
        }

        public async Task<BaseResponse> ConciliarCajaAsync(ConciliarCajaRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CONCILIAR_CAJA";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_corte_id", OracleDbType.Int32, request.CorteId, ParameterDirection.Input);
            cmd.Parameters.Add("p_observacion", OracleDbType.Varchar2, request.Observacion, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? "ERROR",
                Mensaje = pMensaje.Value?.ToString() ?? "Error desconocido al conciliar la caja"
            };
        }
    }
}
