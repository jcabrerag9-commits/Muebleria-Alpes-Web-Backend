using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Departamento;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos;

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public DepartamentoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateDepartamentoDTO dto)
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
                "PKG_RH_CATALOGOS.SP_CREAR_DEPARTAMENTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado"),
                Mensaje = parameters.Get<string>("p_mensaje"),
                Id = parameters.Get<int?>("p_id")
            };
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateDepartamentoDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_nombre", dto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_descripcion", dto.Descripcion, OracleDbType.Varchar2, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_CATALOGOS.SP_ACTUALIZAR_DEPARTAMENTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado"),
                Mensaje = parameters.Get<string>("p_mensaje")
            };
        }

        public async Task<ResponseDepartamentoDTO?> ObtenerPorIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var result = await connection.QueryFirstOrDefaultAsync<Departamento>(
                "PKG_RH_CATALOGOS.SP_OBTENER_DEPARTAMENTO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
                return null;

            return new ResponseDepartamentoDTO
            {
                Id = result.RHD_DEPARTAMENTO,
                Codigo = result.RHD_CODIGO,
                Nombre = result.RHD_NOMBRE,
                Descripcion = result.RHD_DESCRIPCION
            };
        }

        public async Task<IEnumerable<ResponseDepartamentoDTO>> ListarAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var result = await connection.QueryAsync<Departamento>(
                "PKG_RH_CATALOGOS.SP_LISTAR_DEPARTAMENTOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.Select(x => new ResponseDepartamentoDTO
            {
                Id = x.RHD_DEPARTAMENTO,
                Codigo = x.RHD_CODIGO,
                Nombre = x.RHD_NOMBRE,
                Descripcion = x.RHD_DESCRIPCION
            });
        }
    }
}
