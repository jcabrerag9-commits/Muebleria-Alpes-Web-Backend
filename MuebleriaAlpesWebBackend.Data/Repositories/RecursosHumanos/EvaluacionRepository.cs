using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Evaluacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using Oracle.ManagedDataAccess.Client;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class EvaluacionRepository : IEvaluacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public EvaluacionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CrearEvaluacionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha", dto.Fecha, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_calificacion", dto.Calificacion, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_comentarios", dto.Comentarios, OracleDbType.Clob, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_EVALUACION.SP_CREAR_EVALUACION",
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

        public async Task<ResponseSpDTO> ActualizarAsync(int id, ActualizarEvaluacionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_calificacion", dto.Calificacion, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_comentarios", dto.Comentarios, OracleDbType.Clob, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_EVALUACION.SP_ACTUALIZAR_EVALUACION",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<EvaluacionResponseDTO?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entity = await connection.QueryFirstOrDefaultAsync<Evaluacion>(
                "PKG_RH_EVALUACION.SP_OBTENER_EVALUACION",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (entity == null)
                return null;

            return MapToResponse(entity);
        }

        public async Task<IEnumerable<EvaluacionResponseDTO>> ListarPorEmpleadoAsync(int empleadoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Evaluacion>(
                "PKG_RH_EVALUACION.SP_LISTAR_EVALUACIONES_EMPLEADO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(MapToResponse);
        }

        public async Task<IEnumerable<EvaluacionResponseDTO>> ReporteAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_fecha_inicio", fechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", fechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Evaluacion>(
                "PKG_RH_EVALUACION.SP_REPORTE_EVALUACIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(MapToResponse);
        }

        public async Task<PromedioEvaluacionResponseDTO> ObtenerPromedioAsync(int empleadoId, DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", fechaInicio, DbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", fechaFin, DbType.Date, ParameterDirection.Input);

            var promedio = await connection.ExecuteScalarAsync<decimal>(
                @"SELECT PKG_RH_EVALUACION.FN_PROMEDIO_EVALUACIONES(
                    :p_empleado_id,
                    :p_fecha_inicio,
                    :p_fecha_fin
                ) FROM DUAL",
                parameters
            );

            return new PromedioEvaluacionResponseDTO
            {
                EmpleadoId = empleadoId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Promedio = promedio
            };
        }

        private static EvaluacionResponseDTO MapToResponse(Evaluacion entity)
        {
            return new EvaluacionResponseDTO
            {
                Id = entity.EVL_EVALUACION,
                EmpleadoId = entity.EMP_EMPLEADO,
                CodigoEmpleado = entity.EMP_CODIGO,
                NombreEmpleado = entity.NOMBRE_EMPLEADO,
                Fecha = entity.EVL_FECHA_EVALUACION,
                Calificacion = entity.EVL_CALIFICACION,
                Comentarios = entity.EVL_COMENTARIOS
            };
        }
    }
}
