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
using System.Threading;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ProductoImagenRepository : BaseRepository<ProductoImagenRepository>, IProductoImagenRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoImagenRepository(OracleConnectionFactory connectionFactory, ILogger<ProductoImagenRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            LogOperacionInicio(nameof(SubirImagenAsync), new { productoId, nombre, contentType });

            var p = new OracleDynamicParameters();
            p.Add("p_pro_producto", productoId);
            p.Add("p_pim_archivo", archivo, dbType: OracleDbType.Blob);
            p.Add("p_pim_nombre", nombre);
            p.Add("p_pim_content_type", contentType);
            p.Add("p_pim_tamanio", tamanio);
            p.Add("p_pim_url", string.IsNullOrWhiteSpace(url) ? "LOCAL_BLOB" : url);
            p.Add("p_pim_tipo", tipo);
            p.Add("p_pim_orden", orden);
            p.Add("p_id_generado", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;
            
            await activeConnection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTO_IMAGEN.sp_subir_imagen_producto", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            int id = p.Get<int>("p_id_generado");
            
            LogOperacionExito(nameof(SubirImagenAsync), id);
            return id;
        }

        public async Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_pim_id", imagenId);
                p.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[IMAGEN REPOS] Obteniendo binario de imagen {ImagenId}...", imagenId);

                var img = await connection.QueryFirstOrDefaultAsync<ProductoImagenDTO>(new CommandDefinition(
                    "PKG_PRODUCTO_IMAGEN.sp_obtener_imagen_producto",
                    p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));

                if (img != null)
                {
                    _logger.LogInformation("[IMAGEN REPOS] Imagen encontrada: {Nombre}, Tamaño: {Size} bytes, Archivo es null? {IsNull}", 
                        img.NombreArchivo, img.Tamanio, img.Archivo == null);
                }
                else
                {
                    _logger.LogWarning("[IMAGEN REPOS] Imagen {ImagenId} no encontrada.", imagenId);
                }

                return img;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMAGEN REPOS] Error en ObtenerImagenAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_pro_producto", productoId);
                p.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[IMAGEN REPOS] Listando imágenes para producto {ProductoId}...", productoId);
                
                var results = await connection.QueryAsync<ProductoImagenListadoDTO>(new CommandDefinition(
                    "PKG_PRODUCTO_IMAGEN.sp_listar_imagenes_producto",
                    p,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[IMAGEN REPOS] OK: {Count} imágenes encontradas.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMAGEN REPOS] Error en ListarPorProductoAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId, CancellationToken ct = default)
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto_id", productoId);
                p.Add("p_imagen_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[IMAGEN REPOS] Buscando imagen principal para producto {ProductoId}...", productoId);

                await connection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTO_IMAGEN.sp_obtener_imagen_principal", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));

                int? imagenId = p.Get<int?>("p_imagen_id");
                if (imagenId.HasValue && imagenId > 0)
                {
                    _logger.LogInformation("[IMAGEN REPOS] Imagen principal encontrada: ID {ImagenId}", imagenId);
                    return await ObtenerImagenAsync(imagenId.Value, ct);
                }

                _logger.LogWarning("[IMAGEN REPOS] El producto {ProductoId} no tiene imagen principal.", productoId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[IMAGEN REPOS] SP no disponible ({Message}). Intentando consulta directa...", ex.Message);
                // Fallback: PKG_PRODUCTO_IMAGEN no está compilado — consulta directa a ALP_PRODUCTO_IMAGEN
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    const string sql = @"
                        SELECT PIM_PRODUCTO_IMAGEN AS ImagenId,
                               PRO_PRODUCTO        AS ProductoId,
                               PIM_URL             AS Url,
                               PIM_TIPO            AS Tipo,
                               PIM_ORDEN           AS Orden
                        FROM (
                            SELECT PIM_PRODUCTO_IMAGEN, PRO_PRODUCTO, PIM_URL, PIM_TIPO, PIM_ORDEN
                            FROM ALP_PRODUCTO_IMAGEN
                            WHERE PRO_PRODUCTO = :productoId
                              AND PIM_TIPO     = 'PRINCIPAL'
                              AND PIM_ESTADO   = 'ACTIVO'
                            ORDER BY PIM_ORDEN ASC
                        )
                        WHERE ROWNUM = 1";

                    var img = await connection.QueryFirstOrDefaultAsync<ProductoImagenDTO>(sql, new { productoId });
                    if (img != null)
                    {
                        img.NombreArchivo = $"producto_{productoId}.jpg";
                        img.ContentType   = "image/jpeg";
                        _logger.LogInformation("[IMAGEN REPOS] Fallback: URL encontrada para producto {ProductoId}: {Url}", productoId, img.Url);
                    }
                    return img;
                }
                catch (Exception exFallback)
                {
                    _logger.LogError(exFallback, "[IMAGEN REPOS] Fallback directo también falló: {Message}", exFallback.Message);
                    return null;
                }
            }
        }

        public async Task<bool> EliminarImagenAsync(int imagenId, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            LogOperacionInicio(nameof(EliminarImagenAsync), imagenId);

            var p = new OracleDynamicParameters();
            p.Add("p_pim_id", imagenId);
            p.Add("p_filas_afect", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            await activeConnection.ExecuteAsync(new CommandDefinition("PKG_PRODUCTO_IMAGEN.sp_eliminar_imagen_producto", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            
            bool exito = p.Get<int>("p_filas_afect") > 0;
            LogOperacionExito(nameof(EliminarImagenAsync), exito);
            return exito;
        }
    }
}
