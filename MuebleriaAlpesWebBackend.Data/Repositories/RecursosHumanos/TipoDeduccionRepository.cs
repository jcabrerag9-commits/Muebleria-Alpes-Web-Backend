using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.TipoDeduccion;
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
    public class TipoDeduccionRepository : ITipoDeduccionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public TipoDeduccionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateTipoDeduccionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_codigo", dto.Codigo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_CREAR_TIPO_DEDUCCION",
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

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateTipoDeduccionDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_ACTUALIZAR_TIPO_DEDUCCION",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty
            };
        }

        public async Task<IEnumerable<ResponseTipoDeduccionDTO>> ListarAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<TipoDeduccion>(
                "PKG_RH_CATALOGOS.SP_LISTAR_TIPOS_DEDUCCION",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(entity => new ResponseTipoDeduccionDTO
            {
                Id = entity.TDE_TIPO_DEDUCCION,
                Codigo = entity.TDE_CODIGO,
                Nombre = entity.TDE_NOMBRE
            });
        }
    }
}
