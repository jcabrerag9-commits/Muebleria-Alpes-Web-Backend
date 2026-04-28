using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.AsignacionesEmpleado;
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
    public class AsignacionEmpleadoRepository : IAsignacionEmpleadoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public AsignacionEmpleadoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> AsignarDepartamentoAsync(AsignarDepartamentoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_depto_id", dto.DepartamentoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", dto.FechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_ASIGNAR_DEPARTAMENTO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty,
                Id = parameters.Get<int?>("p_id")
            };
        }

        public async Task<ResponseSpDTO> FinalizarDepartamentoAsync(int asignacionId, FinalizarAsignacionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_asignacion_id", asignacionId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", dto.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_FINALIZAR_ASIGNACION_DEPTO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<AsignarDepartamentoResponseDTO>> ListarDepartamentosAsync(int empleadoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<AsignacionDepartamento>(
                "PKG_RH_EMPLEADOS.SP_LISTAR_ASIGNACIONES_DEPTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new AsignarDepartamentoResponseDTO
            {
                Id = x.EDE_EMPLEADO_DEPARTAMENTO,
                Departamento = x.DEPARTAMENTO,
                FechaInicio = x.EDE_FECHA_INICIO,
                FechaFin = x.EDE_FECHA_FIN,
                Estado = x.EDE_ESTADO
            });
        }

        public async Task<ResponseSpDTO> AsignarPuestoAsync(AsignarPuestoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_puesto_id", dto.PuestoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_salario", dto.Salario, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", dto.FechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_ASIGNAR_PUESTO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty,
                Id = parameters.Get<int?>("p_id")
            };
        }

        public async Task<ResponseSpDTO> FinalizarPuestoAsync(int asignacionId, FinalizarAsignacionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_asignacion_id", asignacionId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", dto.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_FINALIZAR_ASIGNACION_PUESTO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<HistorialPuestoResponseDTO>> ListarPuestosAsync(int empleadoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<HistorialPuesto>(
                "PKG_RH_EMPLEADOS.SP_LISTAR_HISTORIAL_PUESTOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new HistorialPuestoResponseDTO
            {
                Id = x.EPU_EMPLEADO_PUESTO,
                Puesto = x.PUESTO,
                Salario = x.EPU_SALARIO,
                FechaInicio = x.EPU_FECHA_INICIO,
                FechaFin = x.EPU_FECHA_FIN,
                Estado = x.EPU_ESTADO
            });
        }

        public async Task<ResponseSpDTO> AsignarTurnoAsync(AsignarTurnoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_turno_id", dto.TurnoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", dto.FechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_ASIGNAR_TURNO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty,
                Id = parameters.Get<int?>("p_id")
            };
        }

        public async Task<ResponseSpDTO> FinalizarTurnoAsync(int asignacionId, FinalizarAsignacionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_asignacion_id", asignacionId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", dto.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync("PKG_RH_EMPLEADOS.SP_FINALIZAR_ASIGNACION_TURNO", parameters, commandType: CommandType.StoredProcedure);

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<AsignacionTurnoResponseDTO>> ListarTurnosAsync(int empleadoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_empleado_id", empleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<AsignacionTurno>(
                "PKG_RH_EMPLEADOS.SP_LISTAR_TURNOS_EMPLEADO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new AsignacionTurnoResponseDTO
            {
                Id = x.ETU_EMPLEADO_TURNO,
                Turno = x.TURNO,
                HoraInicio = x.RHT_HORA_INICIO,
                HoraFin = x.RHT_HORA_FIN,
                FechaInicio = x.ETU_FECHA_INICIO,
                FechaFin = x.ETU_FECHA_FIN,
                Estado = x.ETU_ESTADO
            });
        }
    }
}
