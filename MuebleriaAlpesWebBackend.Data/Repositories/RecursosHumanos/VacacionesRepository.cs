using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Vacaciones;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class VacacionesRepository : IVacacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public VacacionesRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> SolicitarAsync(SolicitarVacacionesDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", dto.FechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", dto.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_dias", dto.Dias, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_SOLICITAR_VACACIONES",
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

        public async Task<ResponseSpDTO> AprobarAsync(int id, CambiarEstadoVacacionesDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_vacacion_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_APROBAR_VACACIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseSpDTO> RechazarAsync(int id, CambiarEstadoVacacionesDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_vacacion_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_motivo", dto.Motivo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_RECHAZAR_VACACIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseSpDTO> CancelarAsync(int id, CambiarEstadoVacacionesDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_vacacion_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_motivo", dto.Motivo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_ASISTENCIA.SP_CANCELAR_VACACIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<VacacionResponseDTO>> ListarAsync(int empleadoId, string? estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Vacacion>(
                "PKG_RH_ASISTENCIA.SP_LISTAR_VACACIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new VacacionResponseDTO
            {
                Id = x.VAC_VACACIONES,
                EmpleadoId = x.EMP_EMPLEADO,
                FechaInicio = x.VAC_FECHA_INICIO,
                FechaFin = x.VAC_FECHA_FIN,
                DiasSolicitados = x.VAC_DIAS_SOLICITADOS,
                Estado = x.VAC_ESTADO,
                FechaCreacion = x.VAC_FECHA_CREACION
            });
        }
    }
}
