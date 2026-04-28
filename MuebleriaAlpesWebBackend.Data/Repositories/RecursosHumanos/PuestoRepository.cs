using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Puesto;
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
    public class PuestoRepository : IPuestoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PuestoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreatePuestoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_codigo", dto.Codigo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_descripcion", dto.Descripcion, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_CREAR_PUESTO",
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

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdatePuestoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_descripcion", dto.Descripcion, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_ACTUALIZAR_PUESTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoPuestoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_CAMBIAR_ESTADO_PUESTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<ResponsePuestoDTO?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entity = await connection.QueryFirstOrDefaultAsync<Puesto>(
                "PKG_RH_CATALOGOS.SP_OBTENER_PUESTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (entity == null)
                return null;

            return new ResponsePuestoDTO
            {
                Id = entity.RHP_PUESTO,
                Codigo = entity.RHP_CODIGO,
                Nombre = entity.RHP_NOMBRE,
                Descripcion = entity.RHP_DESCRIPCION,
                Estado = entity.RHP_ESTADO
            };
        }

        public async Task<IEnumerable<ResponsePuestoDTO>> ListarAsync(bool soloActivos)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_solo_activos", soloActivos ? "S" : "N", OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Puesto>(
                "PKG_RH_CATALOGOS.SP_LISTAR_PUESTOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(entity => new ResponsePuestoDTO
            {
                Id = entity.RHP_PUESTO,
                Codigo = entity.RHP_CODIGO,
                Nombre = entity.RHP_NOMBRE,
                Descripcion = entity.RHP_DESCRIPCION,
                Estado = entity.RHP_ESTADO
            });
        }
    }
}
