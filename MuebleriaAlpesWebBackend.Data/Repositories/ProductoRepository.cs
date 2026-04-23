using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT PRO_PRODUCTO as Id, TMU_TIPO_MUEBLE as TipoMueble, PRO_SKU as Sku, PRO_NOMBRE as Nombre, PRO_DESCRIPCION_CORTA as DescripcionCorta, PRO_DESCRIPCION_LARGA as DescripcionLarga, PRO_PESO as Peso, PRO_ES_CONFIGURABLE as EsConfigurable, PRO_ESTADO as Estado, PRO_PUBLICADO as Publicado FROM ALP_PRODUCTO";
            return await connection.QueryAsync<Producto>(query);
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT PRO_PRODUCTO as Id, TMU_TIPO_MUEBLE as TipoMueble, PRO_SKU as Sku, PRO_NOMBRE as Nombre, PRO_DESCRIPCION_CORTA as DescripcionCorta, PRO_DESCRIPCION_LARGA as DescripcionLarga, PRO_PESO as Peso, PRO_ES_CONFIGURABLE as EsConfigurable, PRO_ESTADO as Estado, PRO_PUBLICADO as Publicado FROM ALP_PRODUCTO WHERE PRO_PRODUCTO = :id";
            return await connection.QueryFirstOrDefaultAsync<Producto>(query, new { id });
        }

        public async Task<int> CreateAsync(Producto producto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_tipo_mueble", producto.TipoMueble);
            parameters.Add("p_sku", producto.Sku);
            parameters.Add("p_nombre", producto.Nombre);
            parameters.Add("p_desc_corta", producto.DescripcionCorta);
            parameters.Add("p_desc_larga", producto.DescripcionLarga);
            parameters.Add("p_peso", producto.Peso);
            parameters.Add("p_es_configurable", producto.EsConfigurable);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_crear_producto", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_id_nuevo");
        }

        public async Task UpdateAsync(Producto producto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", producto.Id);
            parameters.Add("p_nombre", producto.Nombre);
            parameters.Add("p_desc_corta", producto.DescripcionCorta);
            parameters.Add("p_desc_larga", producto.DescripcionLarga);
            parameters.Add("p_peso", producto.Peso);
            parameters.Add("p_es_configurable", producto.EsConfigurable);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_actualizar_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task ChangeStatusAsync(int id, string estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", id);
            parameters.Add("p_estado", estado);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_cambiar_estado_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteLogicoAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", id);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_eliminar_producto_logico", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpsertDimensionAsync(DimensionProducto dimension)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", dimension.ProductoId);
            parameters.Add("p_alto", dimension.Alto);
            parameters.Add("p_ancho", dimension.Ancho);
            parameters.Add("p_largo", dimension.Largo);
            parameters.Add("p_unidad", dimension.Unidad);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_registrar_dimension_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AsignarCategoriaAsync(int productoId, int categoriaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_categoria", categoriaId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_asignar_categoria_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task QuitarCategoriaAsync(int productoId, int categoriaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_categoria", categoriaId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_quitar_categoria_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AsignarColeccionAsync(int productoId, int coleccionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_coleccion", coleccionId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_asignar_coleccion_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task QuitarColeccionAsync(int productoId, int coleccionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_coleccion", coleccionId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_quitar_coleccion_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AsignarMaterialAsync(int productoId, int materialId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_material", materialId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_asignar_material_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task QuitarMaterialAsync(int productoId, int materialId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_material", materialId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_quitar_material_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AsignarColorAsync(int productoId, int colorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_color", colorId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_asignar_color_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task QuitarColorAsync(int productoId, int colorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_color", colorId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_quitar_color_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateResenaAsync(ResenaProducto resena)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", resena.ProductoId);
            parameters.Add("p_cliente", resena.ClienteId);
            parameters.Add("p_calificacion", resena.Calificacion);
            parameters.Add("p_titulo", resena.Titulo);
            parameters.Add("p_comentario", resena.Comentario);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_registrar_resena_producto", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_id_nuevo");
        }

        public async Task AprobarResenaAsync(int resenaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_resena", resenaId);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_aprobar_resena_producto", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
