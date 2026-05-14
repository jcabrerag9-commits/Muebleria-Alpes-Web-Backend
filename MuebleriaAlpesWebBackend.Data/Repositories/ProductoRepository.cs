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

        public async Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(CrearProductoAsync), new { request.Nombre, request.TipoMueble });

            var p = new OracleDynamicParameters();
            p.Add("p_tipo_mueble", request.TipoMueble);
            p.Add("p_nombre", request.Nombre);
            p.Add("p_desc_corta", request.DescripcionCorta);
            p.Add("p_desc_larga", request.DescripcionLarga);
            p.Add("p_peso", request.Peso);
            p.Add("p_es_configurable", request.EsConfigurable);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;
                
                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTOS.sp_crear_producto", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");
                
                LogOperacionExito(nameof(CrearProductoAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Producto creado con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, ActualizarProductoRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(ActualizarProductoAsync), new { productoId, request.Nombre });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", productoId);
            p.Add("p_nombre", request.Nombre);
            p.Add("p_desc_corta", request.DescripcionCorta);
            p.Add("p_peso", request.Peso);
            p.Add("p_tipo_mueble", request.TipoMueble);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;
                
                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTOS.sp_actualizar_producto", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                LogOperacionExito(nameof(ActualizarProductoAsync));
                return new InventarioResponse<bool> { Resultado = "EXITO", Mensaje = "Producto actualizado con éxito", Data = true };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<bool>(ex, false);
            }
        }

        public async Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(CrearVarianteAsync), new { request.ProductoId, request.Nombre });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", request.ProductoId);
            p.Add("p_nombre", request.Nombre);
            p.Add("p_cod_barras", request.CodigoBarras);
            p.Add("p_imagen_url", request.ImagenUrl);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTO_VARIANTES.sp_crear_variante_producto", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");

                LogOperacionExito(nameof(CrearVarianteAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Variante creada con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto_id", productoId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[PROD REPOS] Obteniendo producto {ProductoId}...", productoId);

                var producto = await connection.QueryFirstOrDefaultAsync<ProductoDTO>(new CommandDefinition(
                    "PKG_PRODUCTOS.sp_obtener_producto_por_id",
                    p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));

                if (producto != null)
                    _logger.LogInformation("[PROD REPOS] Producto {ProductoId} encontrado: {Nombre}", productoId, producto.Nombre);
                else
                    _logger.LogWarning("[PROD REPOS] Producto {ProductoId} NO encontrado.", productoId);

                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PROD REPOS] Error en ObtenerProductoPorIdAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync(CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[REPOS] Ejecutando PKG_PRODUCTOS.sp_obtener_todos_productos...");
                
                var results = await connection.QueryAsync<ProductoDTO>(new CommandDefinition(
                    "PKG_PRODUCTOS.sp_obtener_todos_productos",
                    p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[REPOS] OK: Se obtuvieron {Count} productos", list.Count);
                return list;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "[REPOS] ERROR ORACLE en ObtenerTodosAsync: {Message}", ex.Message);
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "[REPOS] ERROR GENERAL en ObtenerTodosAsync: {Message}", ex.Message);
                throw;
            }
        }
    }
}
