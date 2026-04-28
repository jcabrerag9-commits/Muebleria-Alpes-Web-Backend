using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
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
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public EmpleadoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateEmpleadoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_codigo", dto.Codigo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_tipo_doc", dto.TipoDocumentoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_num_doc", dto.NumeroDocumento, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_primer_nombre", dto.PrimerNombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_segundo_nombre", dto.SegundoNombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_primer_apellido", dto.PrimerApellido, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_segundo_apellido", dto.SegundoApellido, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_email", dto.Email, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_telefono", dto.Telefono, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_fecha_nac", dto.FechaNacimiento, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_ingreso", dto.FechaIngreso, OracleDbType.Date, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_EMPLEADOS.SP_CREAR_EMPLEADO",
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

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateEmpleadoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_email", dto.Email, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_telefono", dto.Telefono, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_EMPLEADOS.SP_ACTUALIZAR_EMPLEADO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoEmpleadoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_motivo", dto.Motivo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_EMPLEADOS.SP_CAMBIAR_ESTADO_EMPLEADO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseEmpleadoDTO?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entity = await connection.QueryFirstOrDefaultAsync<Empleado>(
                "PKG_RH_EMPLEADOS.SP_OBTENER_EMPLEADO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (entity == null)
                return null;

            return MapToResponse(entity);
        }

        public async Task<IEnumerable<ResponseEmpleadoDTO>> ListarAsync(string? estado)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_estado", estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Empleado>(
                "PKG_RH_EMPLEADOS.SP_LISTAR_EMPLEADOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(MapToResponse);
        }

        private static ResponseEmpleadoDTO MapToResponse(Empleado entity)
        {
            var nombreCompleto = !string.IsNullOrWhiteSpace(entity.NOMBRE_COMPLETO)
                ? entity.NOMBRE_COMPLETO
                : $"{entity.EMP_PRIMER_NOMBRE} {entity.EMP_PRIMER_APELLIDO}".Trim();

            return new ResponseEmpleadoDTO
            {
                Id = entity.EMP_EMPLEADO,
                Codigo = entity.EMP_CODIGO,
                TipoDocumento = entity.TIPO_DOCUMENTO,
                NumeroDocumento = entity.EMP_NUMERO_DOCUMENTO,
                PrimerNombre = entity.EMP_PRIMER_NOMBRE,
                SegundoNombre = entity.EMP_SEGUNDO_NOMBRE,
                PrimerApellido = entity.EMP_PRIMER_APELLIDO,
                SegundoApellido = entity.EMP_SEGUNDO_APELLIDO,
                NombreCompleto = nombreCompleto,
                Email = entity.EMP_EMAIL,
                Telefono = entity.EMP_TELEFONO,
                FechaNacimiento = entity.EMP_FECHA_NACIMIENTO,
                FechaIngreso = entity.EMP_FECHA_INGRESO,
                Estado = entity.EMP_ESTADO
            };
        }
    }
}
