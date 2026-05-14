using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Turno;
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

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public TurnoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateTurnoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_hora_inicio", dto.HoraInicio, OracleDbType.TimeStamp, ParameterDirection.Input);
            parameters.Add("p_hora_fin", dto.HoraFin, OracleDbType.TimeStamp, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_CREAR_TURNO",
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

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTurnoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_hora_inicio", dto.HoraInicio, OracleDbType.TimeStamp, ParameterDirection.Input);
            parameters.Add("p_hora_fin", dto.HoraFin, OracleDbType.TimeStamp, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_ACTUALIZAR_TURNO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoTurnoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_CAMBIAR_ESTADO_TURNO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseTurnoDTO?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entity = await connection.QueryFirstOrDefaultAsync<Turno>(
                "PKG_RH_CATALOGOS.SP_OBTENER_TURNO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (entity == null)
                return null;

            return new ResponseTurnoDTO
            {
                Id = entity.RHT_TURNO,
                Nombre = entity.RHT_NOMBRE,
                HoraInicio = entity.RHT_HORA_INICIO,
                HoraFin = entity.RHT_HORA_FIN,
                Estado = entity.RHT_ESTADO
            };
        }

        public async Task<IEnumerable<ResponseTurnoDTO>> ListarAsync(bool soloActivos)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_solo_activos", soloActivos ? "S" : "N", OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Turno>(
                "PKG_RH_CATALOGOS.SP_LISTAR_TURNOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(entity => new ResponseTurnoDTO
            {
                Id = entity.RHT_TURNO,
                Nombre = entity.RHT_NOMBRE,
                HoraInicio = entity.RHT_HORA_INICIO,
                HoraFin = entity.RHT_HORA_FIN,
                Estado = entity.RHT_ESTADO
            });
        }
    }
}
