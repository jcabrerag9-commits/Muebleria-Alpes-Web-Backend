using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ProductoInventarioRepository : IProductoInventarioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoInventarioRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_tipo_mueble", request.TipoMueble, DbType.Int32);
            parameters.Add("p_nombre", request.Nombre, DbType.String);
            parameters.Add("p_desc_corta", request.DescripcionCorta, DbType.String);
            parameters.Add("p_desc_larga", request.DescripcionLarga, DbType.String);
            parameters.Add("p_peso", request.Peso, DbType.Decimal);
            parameters.Add("p_es_configurable", request.EsConfigurable, DbType.String);
            
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;
            
            await activeConnection.ExecuteAsync("PKG_PRODUCTOS.sp_crear_producto", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<int?>
            {
                Resultado = "EXITO",
                Mensaje = "Producto creado con éxito",
                Data = parameters.Get<int?>("p_id_nuevo")
            };
        }

        public async Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, object request, IDbTransaction? transaction = null)
        {
            // Implementación simplificada para el ejemplo
            return new InventarioResponse<bool> { Resultado = "EXITO", Data = true };
        }

        public async Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", request.ProductoId, DbType.Int32);
            parameters.Add("p_nombre", request.Nombre, DbType.String);
            parameters.Add("p_cod_barras", request.CodigoBarras, DbType.String);
            parameters.Add("p_imagen_url", request.ImagenUrl, DbType.String);
            
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_PRODUCTO_VARIANTES.sp_crear_variante_producto", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<int?>
            {
                Resultado = "EXITO",
                Mensaje = "Variante creada con éxito",
                Data = parameters.Get<int?>("p_id_nuevo")
            };
        }

        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT PRO_PRODUCTO as ProductoId, 
                                 PRO_SKU as Sku, 
                                 PRO_NOMBRE as Nombre, 
                                 PRO_DESCRIPCION_CORTA as DescripcionCorta, 
                                 PRO_DESCRIPCION_LARGA as DescripcionLarga,
                                 PRO_PESO as Peso,
                                 PRO_ES_CONFIGURABLE as EsConfigurable,
                                 PRO_ESTADO as Estado,
                                 PRO_FECHA_CREACION as FechaRegistro
                          FROM ALP_PRODUCTO 
                          WHERE PRO_PRODUCTO = :productoId";
            return await connection.QueryFirstOrDefaultAsync<ProductoDTO>(sql, new { productoId });
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT PRO_PRODUCTO as ProductoId, 
                                 PRO_SKU as Sku, 
                                 PRO_NOMBRE as Nombre, 
                                 PRO_DESCRIPCION_CORTA as DescripcionCorta, 
                                 PRO_DESCRIPCION_LARGA as DescripcionLarga,
                                 PRO_PESO as Peso,
                                 PRO_ES_CONFIGURABLE as EsConfigurable,
                                 PRO_ESTADO as Estado,
                                 PRO_FECHA_CREACION as FechaRegistro
                          FROM ALP_PRODUCTO 
                          ORDER BY PRO_FECHA_CREACION DESC";
            return await connection.QueryAsync<ProductoDTO>(sql);
        }
    }

    public class InventarioRepository : IInventarioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public InventarioRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", request.ProductoId, DbType.Int32);
            parameters.Add("p_variante", request.VarianteId, DbType.Int32);
            parameters.Add("p_bodega", request.BodegaId, DbType.Int32);
            parameters.Add("p_cantidad", request.Cantidad, DbType.Int32);
            parameters.Add("p_costo_unitario", request.CostoUnitario, DbType.Decimal);
            parameters.Add("p_observacion", request.Observacion, DbType.String);
            parameters.Add("p_usuario", request.UsuarioId, DbType.Int32);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_INVENTARIO.sp_registrar_entrada_inventario", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<int?>
            {
                Resultado = "EXITO",
                Mensaje = "Entrada registrada con éxito",
                Data = parameters.Get<int?>("p_id_nuevo")
            };
        }

        public async Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", request.ProductoId, DbType.Int32);
            parameters.Add("p_variante", request.VarianteId, DbType.Int32);
            parameters.Add("p_bodega", request.BodegaId, DbType.Int32);
            parameters.Add("p_cantidad", request.Cantidad, DbType.Int32);
            parameters.Add("p_orden_venta", request.OrdenVentaId, DbType.Int32);
            parameters.Add("p_observacion", request.Observacion, DbType.String);
            parameters.Add("p_usuario", request.UsuarioId, DbType.Int32);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_INVENTARIO.sp_registrar_salida_inventario", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<int?>
            {
                Resultado = "EXITO",
                Mensaje = "Salida registrada con éxito",
                Data = parameters.Get<int?>("p_id_nuevo")
            };
        }

        public async Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", request.ProductoId, DbType.Int32);
            parameters.Add("p_variante", request.VarianteId, DbType.Int32);
            parameters.Add("p_bodega", request.BodegaId, DbType.Int32);
            parameters.Add("p_cliente", request.ClienteId, DbType.Int32);
            parameters.Add("p_cantidad", request.Cantidad, DbType.Int32);
            parameters.Add("p_motivo", request.Motivo, DbType.String);
            parameters.Add("p_expiracion", request.Expiracion, DbType.DateTime);
            parameters.Add("p_usuario", request.UsuarioId, DbType.Int32);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_EXISTENCIAS.sp_reservar_stock", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<int?>
            {
                Resultado = "EXITO",
                Data = parameters.Get<int?>("p_id_nuevo")
            };
        }

        public async Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_reserva", reservaId, DbType.Int32);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_EXISTENCIAS.sp_liberar_stock_reservado", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<bool> { Resultado = "EXITO", Data = true };
        }

        public async Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT PRO_PRODUCTO as ProductoId, 
                                 PVA_PRODUCTO_VARIANTE as VarianteId, 
                                 BOD_BODEGA as BodegaId, 
                                 EXI_CANTIDAD_DISPONIBLE as CantidadDisponible, 
                                 EXI_CANTIDAD_RESERVADA as CantidadReservada,
                                 EXI_ULTIMA_ACTUALIZACION as UltimaActualizacion
                          FROM ALP_EXISTENCIA 
                          WHERE PRO_PRODUCTO = :productoId";
            return await connection.QueryAsync<ExistenciaDTO>(sql, new { productoId });
        }

        public async Task<InventarioResponse<bool>> RegistrarAjusteAsync(AjusteStockRequest request, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", request.ProductoId, DbType.Int32);
            parameters.Add("p_variante", request.VarianteId, DbType.Int32);
            parameters.Add("p_bodega", request.BodegaId, DbType.Int32);
            parameters.Add("p_cantidad_nueva", request.CantidadNueva, DbType.Int32);
            parameters.Add("p_motivo", request.Motivo, DbType.String);
            parameters.Add("p_usuario", request.UsuarioId, DbType.Int32);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync("PKG_INVENTARIO.sp_registrar_ajuste_stock", parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

            return new InventarioResponse<bool>
            {
                Resultado = "EXITO",
                Mensaje = "Ajuste registrado con éxito",
                Data = true
            };
        }
    }
}
