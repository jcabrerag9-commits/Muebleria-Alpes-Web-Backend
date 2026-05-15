using Dapper;
using Microsoft.Extensions.Logging;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ProductoRepository : BaseRepository<ProductoRepository>, IProductoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoRepository(OracleConnectionFactory connectionFactory, ILogger<ProductoRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            _logger.LogInformation("[AUDITORÍA] Iniciando GetAllAsync en ProductoRepository");
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var result = await connection.QueryAsync<dynamic>(
                "PKG_PRODUCTOS.sp_obtener_todos_productos",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var list = result.Select(d => new Producto
            {
                Id = d.PRODUCTOID != null ? (int)d.PRODUCTOID : 0,
                Sku = d.SKU?.ToString(),
                Nombre = d.NOMBRE?.ToString() ?? "SIN NOMBRE",
                DescripcionCorta = d.DESCRIPCIONCORTA?.ToString(),
                DescripcionLarga = d.DESCRIPCIONLARGA?.ToString() ?? "",
                Peso = d.PESO != null ? (decimal?)Convert.ToDecimal(d.PESO) : null,
                EsConfigurable = d.ESCONFIGURABLE?.ToString() ?? "N",
                Estado = d.ESTADO?.ToString() ?? "DESCONOCIDO",
                TipoMueble = d.TIPOMUEBLEID != null ? (int)d.TIPOMUEBLEID : 0
            }).ToList();

            _logger.LogInformation("[AUDITORÍA] GetAllAsync recuperó {Count} productos desde PKG_PRODUCTOS", list.Count);
            return list;
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            _logger.LogInformation("[AUDITORÍA] GetByIdAsync para ID: {Id}", id);
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_producto_id", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var d = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "PKG_PRODUCTOS.sp_obtener_producto_por_id",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (d == null) 
            {
                _logger.LogWarning("[AUDITORÍA] Producto ID {Id} no encontrado en Oracle", id);
                return null!;
            }

            return new Producto
            {
                Id = (int)d.PRODUCTOID,
                Sku = d.SKU?.ToString(),
                Nombre = d.NOMBRE?.ToString() ?? "SIN NOMBRE",
                DescripcionCorta = d.DESCRIPCIONCORTA?.ToString(),
                DescripcionLarga = d.DESCRIPCIONLARGA?.ToString() ?? "",
                Peso = d.PESO != null ? (decimal?)Convert.ToDecimal(d.PESO) : null,
                EsConfigurable = d.ESCONFIGURABLE?.ToString() ?? "N",
                Estado = d.ESTADO?.ToString() ?? "DESCONOCIDO",
                TipoMueble = (int)d.TIPOMUEBLEID
            };
        }

        public async Task CreateAsync(Producto producto)
        {
            _logger.LogInformation("[AUDITORÍA] Intentando SP_CREAR_PRODUCTO para: {Nombre}", producto.Nombre);
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            
            parameters.Add("p_tipo_mueble", producto.TipoMueble, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_nombre", producto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_desc_corta", producto.DescripcionCorta, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_desc_larga", producto.DescripcionLarga, OracleDbType.Clob, ParameterDirection.Input);
            parameters.Add("p_peso", producto.Peso, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_es_configurable", producto.EsConfigurable, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_id_nuevo", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            try
            {
                await connection.ExecuteAsync("PKG_PRODUCTOS.sp_crear_producto", parameters, commandType: CommandType.StoredProcedure);
                producto.Id = parameters.Get<int>("p_id_nuevo");
                _logger.LogInformation("[AUDITORÍA] Producto creado exitosamente con ID: {Id}", producto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR CRÍTICO] Fallo en SP_CREAR_PRODUCTO");
                throw;
            }
        }

        public async Task UpdateAsync(Producto producto)
        {
            _logger.LogInformation("[AUDITORÍA] Iniciando UPDATE para Producto ID: {Id}. Datos: {Nombre}, TM: {TipoMueble}", producto.Id, producto.Nombre, producto.TipoMueble);
            try 
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new OracleDynamicParameters();
                
                parameters.Add("p_producto", producto.Id, OracleDbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nombre", producto.Nombre, OracleDbType.Varchar2, ParameterDirection.Input);
                parameters.Add("p_desc_corta", producto.DescripcionCorta, OracleDbType.Varchar2, ParameterDirection.Input);
                parameters.Add("p_desc_larga", producto.DescripcionLarga, OracleDbType.Clob, ParameterDirection.Input);
                parameters.Add("p_peso", producto.Peso, OracleDbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_es_configurable", producto.EsConfigurable, OracleDbType.Varchar2, ParameterDirection.Input);
                parameters.Add("p_tipo_mueble", producto.TipoMueble, OracleDbType.Int32, ParameterDirection.Input);

                await connection.ExecuteAsync("PKG_PRODUCTOS.sp_actualizar_producto", parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("[AUDITORÍA] UPDATE exitoso para ID: {Id}", producto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR CRÍTICO] Fallo en sp_actualizar_producto para ID {Id}: {Message}", producto.Id, ex.Message);
                throw;
            }
        }

        public async Task ChangeStatusAsync(int id, string estado)
        {
            _logger.LogInformation("[AUDITORÍA] Cambiando estado de Producto ID {Id} a {Estado}", id, estado);
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_producto", id, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", estado, OracleDbType.Varchar2, ParameterDirection.Input);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_cambiar_estado_producto", parameters, commandType: CommandType.StoredProcedure);
            _logger.LogInformation("[AUDITORÍA] Estado de Producto ID {Id} cambiado exitosamente", id);
        }

        public async Task DeleteLogicoAsync(int id)
        {
            _logger.LogInformation("[AUDITORÍA] Ejecutando eliminación lógica para Producto ID: {Id}", id);
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_producto", id, OracleDbType.Int32, ParameterDirection.Input);

            await connection.ExecuteAsync("PKG_PRODUCTOS.sp_eliminar_producto_logico", parameters, commandType: CommandType.StoredProcedure);
            _logger.LogInformation("[AUDITORÍA] Producto ID: {Id} eliminado lógicamente", id);
        }

        public async Task UpsertDimensionAsync(DimensionProducto dimension)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", dimension.ProductoId);
            parameters.Add("p_alto", dimension.Alto);
            parameters.Add("p_ancho", dimension.Ancho);
            parameters.Add("p_largo", dimension.Largo);
            parameters.Add("p_unidad", dimension.UnidadId);

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
