using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class VarianteRepository : IVarianteRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public VarianteRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductoVariante>> GetByProductoIdAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT PVA_PRODUCTO_VARIANTE as Id, PRO_PRODUCTO as ProductoId, PVA_SKU as Sku, PVA_NOMBRE as Nombre, PVA_CODIGO_BARRAS as CodigoBarras, PVA_IMAGEN_URL as ImagenUrl, PVA_ESTADO as Estado FROM ALP_PRODUCTO_VARIANTE WHERE PRO_PRODUCTO = :productoId";
            return await connection.QueryAsync<ProductoVariante>(query, new { productoId });
        }

        public async Task<int> CreateAsync(ProductoVariante variante)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", variante.ProductoId);
            parameters.Add("p_sku", variante.Sku);
            parameters.Add("p_nombre", variante.Nombre);
            parameters.Add("p_cod_barras", variante.CodigoBarras);
            parameters.Add("p_imagen_url", variante.ImagenUrl);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_crear_variante_producto", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_id_nuevo");
        }

        public async Task UpdateAsync(ProductoVariante variante)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_variante", variante.Id);
            parameters.Add("p_nombre", variante.Nombre);
            parameters.Add("p_cod_barras", variante.CodigoBarras);
            parameters.Add("p_imagen_url", variante.ImagenUrl);

            await connection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_actualizar_variante_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task ChangeStatusAsync(int id, string estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_variante", id);
            parameters.Add("p_estado", estado);

            await connection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_cambiar_estado_variante_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AsignarAtributoAsync(int varianteId, int atributoValorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_variante", varianteId);
            parameters.Add("p_atributo_valor", atributoValorId);

            await connection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_asignar_atributo_valor_variante", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task QuitarAtributoAsync(int varianteId, int atributoValorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_variante", varianteId);
            parameters.Add("p_atributo_valor", atributoValorId);

            await connection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_quitar_atributo_valor_variante", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
