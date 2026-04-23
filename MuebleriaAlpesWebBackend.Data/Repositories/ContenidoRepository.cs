using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ContenidoRepository : IContenidoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ContenidoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductoImagen>> GetImagenesByProductoIdAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = "SELECT PIM_PRODUCTO_IMAGEN as Id, PRO_PRODUCTO as ProductoId, PIM_URL as Url, PIM_TIPO as Tipo, PIM_ORDEN as Orden, PIM_ESTADO as Estado FROM ALP_PRODUCTO_IMAGEN WHERE PRO_PRODUCTO = :productoId AND PIM_ESTADO = 'ACTIVO' ORDER BY PIM_ORDEN";
            return await connection.QueryAsync<ProductoImagen>(query, new { productoId });
        }

        public async Task<int> CreateImagenAsync(ProductoImagen imagen)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", imagen.ProductoId);
            parameters.Add("p_url", imagen.Url);
            parameters.Add("p_tipo", imagen.Tipo);
            parameters.Add("p_orden", imagen.Orden);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_PRODUCTO_CONTENIDO.sp_agregar_imagen_producto", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_id_nuevo");
        }

        public async Task UpdateImagenAsync(ProductoImagen imagen)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_imagen", imagen.Id);
            parameters.Add("p_url", imagen.Url);
            parameters.Add("p_orden", imagen.Orden);

            await connection.ExecuteAsync("PKG_PRODUCTO_CONTENIDO.sp_actualizar_imagen_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task SetImagenPrincipalAsync(int productoId, int imagenId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", productoId);
            parameters.Add("p_imagen", imagenId);

            await connection.ExecuteAsync("PKG_PRODUCTO_CONTENIDO.sp_marcar_imagen_principal_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteImagenAsync(int imagenId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_imagen", imagenId);

            await connection.ExecuteAsync("PKG_PRODUCTO_CONTENIDO.sp_eliminar_imagen_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpsertTraduccionAsync(ProductoTraduccion traduccion)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", traduccion.ProductoId);
            parameters.Add("p_idioma", traduccion.IdiomaId);
            parameters.Add("p_nombre", traduccion.Nombre);
            parameters.Add("p_desc_corta", traduccion.DescripcionCorta);
            parameters.Add("p_desc_larga", traduccion.DescripcionLarga);

            await connection.ExecuteAsync("PKG_PRODUCTO_CONTENIDO.sp_registrar_traduccion_producto", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
