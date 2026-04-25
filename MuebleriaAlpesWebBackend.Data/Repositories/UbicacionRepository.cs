using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class UbicacionRepository : IUbicacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public UbicacionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Pais>> ListarPaisesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT PAI_PAIS as Id, PAI_CODIGO as Codigo, PAI_NOMBRE as Nombre, PAI_ESTADO as Estado FROM ALP_PAIS ORDER BY PAI_NOMBRE";
            return await connection.QueryAsync<Pais>(query);
        }

        public async Task<int> CrearPaisAsync(Pais pais)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_pai_codigo", pais.Codigo);
            parameters.Add("p_pai_nombre", pais.Nombre);
            parameters.Add("p_pai_estado", pais.Estado);
            parameters.Add("p_pai_pais_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_CREAR_PAIS", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_pai_pais_out");
        }

        public async Task ActualizarPaisAsync(Pais pais)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_pai_pais", pais.Id);
            parameters.Add("p_pai_codigo", pais.Codigo);
            parameters.Add("p_pai_nombre", pais.Nombre);
            parameters.Add("p_pai_estado", pais.Estado);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_ACTUALIZAR_PAIS", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Departamento>> ListarDepartamentosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT d.DEP_DEPARTAMENTO as Id, d.PAI_PAIS as PaisId, d.DEP_CODIGO as Codigo, 
                                    d.DEP_NOMBRE as Nombre, d.DEP_ESTADO as Estado, p.PAI_NOMBRE as NombrePais 
                             FROM ALP_DEPARTAMENTO d 
                             JOIN ALP_PAIS p ON p.PAI_PAIS = d.PAI_PAIS 
                             ORDER BY p.PAI_NOMBRE, d.DEP_NOMBRE";
            return await connection.QueryAsync<Departamento>(query);
        }

        public async Task<int> CrearDepartamentoAsync(Departamento depto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_pai_pais", depto.PaisId);
            parameters.Add("p_dep_codigo", depto.Codigo);
            parameters.Add("p_dep_nombre", depto.Nombre);
            parameters.Add("p_dep_estado", depto.Estado);
            parameters.Add("p_dep_departamento_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_CREAR_DEPARTAMENTO", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_dep_departamento_out");
        }

        public async Task<IEnumerable<Ciudad>> ListarCiudadesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT c.CIU_CIUDAD as Id, c.DEP_DEPARTAMENTO as DepartamentoId, c.CIU_CODIGO as Codigo, 
                                    c.CIU_NOMBRE as Nombre, c.CIU_ESTADO as Estado, d.DEP_NOMBRE as NombreDepartamento, p.PAI_NOMBRE as NombrePais 
                             FROM ALP_CIUDAD c 
                             JOIN ALP_DEPARTAMENTO d ON d.DEP_DEPARTAMENTO = c.DEP_DEPARTAMENTO 
                             JOIN ALP_PAIS p ON p.PAI_PAIS = d.PAI_PAIS 
                             ORDER BY p.PAI_NOMBRE, d.DEP_NOMBRE, c.CIU_NOMBRE";
            return await connection.QueryAsync<Ciudad>(query);
        }

        public async Task<int> CrearCiudadAsync(Ciudad ciudad)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_dep_departamento", ciudad.DepartamentoId);
            parameters.Add("p_ciu_codigo", ciudad.Codigo);
            parameters.Add("p_ciu_nombre", ciudad.Nombre);
            parameters.Add("p_ciu_estado", ciudad.Estado);
            parameters.Add("p_ciu_ciudad_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_CREAR_CIUDAD", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_ciu_ciudad_out");
        }

        public async Task<IEnumerable<Idioma>> ListarIdiomasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT IDI_IDIOMA as Id, IDI_CODIGO as Codigo, IDI_NOMBRE as Nombre, IDI_ESTADO as Estado FROM ALP_IDIOMA ORDER BY IDI_NOMBRE";
            return await connection.QueryAsync<Idioma>(query);
        }

        public async Task<int> CrearIdiomaAsync(Idioma idioma)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_idi_codigo", idioma.Codigo);
            parameters.Add("p_idi_nombre", idioma.Nombre);
            parameters.Add("p_idi_estado", idioma.Estado);
            parameters.Add("p_idi_idioma_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_CREAR_IDIOMA", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_idi_idioma_out");
        }

        public async Task<IEnumerable<Moneda>> ListarMonedasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT MON_MONEDA as Id, MON_CODIGO as Codigo, MON_NOMBRE as Nombre, MON_SIMBOLO as Simbolo, MON_ESTADO as Estado FROM ALP_MONEDA ORDER BY MON_NOMBRE";
            return await connection.QueryAsync<Moneda>(query);
        }

        public async Task<int> CrearMonedaAsync(Moneda moneda)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_mon_codigo", moneda.Codigo);
            parameters.Add("p_mon_nombre", moneda.Nombre);
            parameters.Add("p_mon_simbolo", moneda.Simbolo);
            parameters.Add("p_mon_estado", moneda.Estado);
            parameters.Add("p_mon_moneda_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_UBICACIONES.SP_CREAR_MONEDA", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_mon_moneda_out");
        }
    }
}
