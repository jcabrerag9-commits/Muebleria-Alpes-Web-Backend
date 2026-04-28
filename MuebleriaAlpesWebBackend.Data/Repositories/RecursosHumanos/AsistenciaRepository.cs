using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Asistencia;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public AsistenciaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> RegistrarAsync(RegistrarAsistenciaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha", dto.Fecha, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_hora_entrada", dto.HoraEntrada, OracleDbType.TimeStamp, ParameterDirection.Input);
            parameters.Add("p_hora_salida", dto.HoraSalida, OracleDbType.TimeStamp, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_REGISTRAR_ASISTENCIA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty,
                Id = parameters.Get<int?>("p_id")
            };
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarAsistenciaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_hora_salida", dto.HoraSalida, OracleDbType.TimeStamp, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_ACTUALIZAR_ASISTENCIA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<AsistenciaResponseDTO>> ListarAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", fechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", fechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Asistencia>(
                "PKG_RH_ASISTENCIA.SP_LISTAR_ASISTENCIA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new AsistenciaResponseDTO
            {
                Id = x.RHA_ASISTENCIA,
                EmpleadoId = x.EMP_EMPLEADO,
                Fecha = x.RHA_FECHA,
                HoraEntrada = x.RHA_HORA_ENTRADA,
                HoraSalida = x.RHA_HORA_SALIDA,
                Estado = x.RHA_ESTADO
            });
        }
    }
}
