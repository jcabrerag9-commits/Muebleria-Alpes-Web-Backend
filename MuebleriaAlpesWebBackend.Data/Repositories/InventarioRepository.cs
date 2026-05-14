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
    public class InventarioRepository : BaseRepository<InventarioRepository>, IInventarioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public InventarioRepository(OracleConnectionFactory connectionFactory, ILogger<InventarioRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(RegistrarEntradaAsync), new { request.ProductoId, request.BodegaId, request.Cantidad });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", request.ProductoId);
            p.Add("p_variante", request.VarianteId);
            p.Add("p_bodega", request.BodegaId);
            p.Add("p_cantidad", request.Cantidad);
            p.Add("p_costo_unitario", request.CostoUnitario);
            p.Add("p_observacion", request.Observacion);
            p.Add("p_usuario", request.UsuarioId);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;
                
                _logger.LogInformation("[STOCK] SP=PKG_INVENTARIO.sp_registrar_entrada_inventario Producto={ProductoId} Bodega={BodegaId} Cantidad={Cantidad} Usuario={UsuarioId}", 
                    request.ProductoId, request.BodegaId, request.Cantidad, request.UsuarioId);

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_INVENTARIO.sp_registrar_entrada_inventario", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");
                
                _logger.LogInformation("[STOCK] Resultado Oracle=EXITO ID={Id}", id);
                LogOperacionExito(nameof(RegistrarEntradaAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Entrada registrada con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(RegistrarSalidaAsync), new { request.ProductoId, request.BodegaId, request.Cantidad });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", request.ProductoId);
            p.Add("p_variante", request.VarianteId);
            p.Add("p_bodega", request.BodegaId);
            p.Add("p_cantidad", request.Cantidad);
            p.Add("p_orden_venta", request.OrdenVentaId);
            p.Add("p_observacion", request.Observacion);
            p.Add("p_usuario", request.UsuarioId);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;
                
                _logger.LogInformation("[STOCK] SP=PKG_INVENTARIO.sp_registrar_salida_inventario Producto={ProductoId} Bodega={BodegaId} Cantidad={Cantidad} Usuario={UsuarioId}", 
                    request.ProductoId, request.BodegaId, request.Cantidad, request.UsuarioId);

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_INVENTARIO.sp_registrar_salida_inventario", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");
                
                _logger.LogInformation("[STOCK] Resultado Oracle=EXITO ID={Id}", id);
                LogOperacionExito(nameof(RegistrarSalidaAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Salida registrada con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(ReservarStockAsync), new { request.ProductoId, request.BodegaId, request.Cantidad });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", request.ProductoId);
            p.Add("p_variante", request.VarianteId);
            p.Add("p_bodega", request.BodegaId);
            p.Add("p_cliente", request.ClienteId);
            p.Add("p_cantidad", request.Cantidad);
            p.Add("p_motivo", request.Motivo);
            p.Add("p_expiracion", request.Expiracion);
            p.Add("p_usuario", request.UsuarioId);
            p.Add("p_orden_venta", null);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;

                _logger.LogInformation("[STOCK] SP=PKG_EXISTENCIAS.sp_reservar_stock Producto={ProductoId} Bodega={BodegaId} Cantidad={Cantidad} Usuario={UsuarioId}", 
                    request.ProductoId, request.BodegaId, request.Cantidad, request.UsuarioId);

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_EXISTENCIAS.sp_reservar_stock", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");

                _logger.LogInformation("[STOCK] Resultado Oracle=EXITO ID={Id}", id);
                LogOperacionExito(nameof(ReservarStockAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Reserva creada con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, int? usuarioId = null, string? observacion = null, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            LogOperacionInicio(nameof(LiberarReservaAsync), new { reservaId, usuarioId });

            var p = new OracleDynamicParameters();
            p.Add("p_reserva", reservaId);
            p.Add("p_usuario", usuarioId);
            p.Add("p_observacion", observacion);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;

                _logger.LogInformation("[STOCK] SP=PKG_EXISTENCIAS.sp_liberar_stock_reservado Reserva={ReservaId} Usuario={UsuarioId}", 
                    reservaId, usuarioId);

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_EXISTENCIAS.sp_liberar_stock_reservado", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                LogOperacionExito(nameof(LiberarReservaAsync));
                return new InventarioResponse<bool> { Resultado = "EXITO", Mensaje = "Reserva liberada con éxito", Data = true };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<bool>(ex, false);
            }
        }

        public async Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto_id", productoId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[INV REPOS] Consultando existencias para producto {ProductoId}...", productoId);

                var results = await connection.QueryAsync<ExistenciaDTO>(new CommandDefinition(
                    "PKG_EXISTENCIAS.sp_obtener_existencias_producto", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[INV REPOS] OK: {Count} registros de existencia encontrados.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[INV REPOS] Error en ObtenerExistenciasPorProductoAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<InventarioResponse<bool>> RegistrarAjusteAsync(AjusteStockRequest request, IDbTransaction? transaction = null, CancellationToken ct = default)
        {
            ValidarParametroNulo(request, nameof(request));
            LogOperacionInicio(nameof(RegistrarAjusteAsync), new { request.ProductoId, request.CantidadNueva });

            var p = new OracleDynamicParameters();
            p.Add("p_producto", request.ProductoId);
            p.Add("p_variante", request.VarianteId);
            p.Add("p_bodega", request.BodegaId);
            p.Add("p_cantidad_nueva", request.CantidadNueva);
            p.Add("p_motivo", request.Motivo);
            p.Add("p_usuario", request.UsuarioId);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
                var activeConnection = transaction?.Connection ?? connection!;

                _logger.LogInformation("[STOCK] SP=PKG_INVENTARIO.sp_registrar_ajuste_stock Producto={ProductoId} Bodega={BodegaId} NuevaCant={Cantidad} Usuario={UsuarioId}", 
                    request.ProductoId, request.BodegaId, request.CantidadNueva, request.UsuarioId);

                await activeConnection.ExecuteAsync(new CommandDefinition("PKG_INVENTARIO.sp_registrar_ajuste_stock", p, transaction: transaction, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                LogOperacionExito(nameof(RegistrarAjusteAsync));
                return new InventarioResponse<bool> { Resultado = "EXITO", Mensaje = "Ajuste registrado con éxito", Data = true };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<bool>(ex, false);
            }
        }

        public async Task<IEnumerable<ReservaDTO>> ObtenerReservasPorProductoAsync(int productoId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto_id", productoId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[INV REPOS] Consultando reservas para producto {ProductoId}...", productoId);

                var results = await connection.QueryAsync<ReservaDTO>(new CommandDefinition(
                    "PKG_EXISTENCIAS.sp_obtener_reservas_producto", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[INV REPOS] OK: {Count} reservas encontradas.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[INV REPOS] Error en ObtenerReservasPorProductoAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ReservaDTO?> ObtenerReservaPorIdAsync(int reservaId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_reserva_id", reservaId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[INV REPOS] Obteniendo reserva {ReservaId}...", reservaId);

                var reserva = await connection.QueryFirstOrDefaultAsync<ReservaDTO>(new CommandDefinition(
                    "PKG_EXISTENCIAS.sp_obtener_reserva_por_id", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                if (reserva != null)
                    _logger.LogInformation("[INV REPOS] Reserva {ReservaId} encontrada.", reservaId);
                else
                    _logger.LogWarning("[INV REPOS] Reserva {ReservaId} NO encontrada.", reservaId);

                return reserva;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[INV REPOS] Error en ObtenerReservaPorIdAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<KardexDTO>> ObtenerKardexPorProductoAsync(int productoId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto_id", productoId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[INV REPOS] Consultando Kardex para producto {ProductoId}...", productoId);

                var results = await connection.QueryAsync<KardexDTO>(new CommandDefinition(
                    "PKG_INVENTARIO.sp_obtener_kardex_producto", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[INV REPOS] OK: {Count} movimientos de Kardex encontrados.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[INV REPOS] Error en ObtenerKardexPorProductoAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<KardexDTO>> ObtenerMovimientosGlobalesAsync(MovimientoFiltroRequest filtro, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_producto", filtro.ProductoId);
                p.Add("p_bodega", filtro.BodegaId);
                p.Add("p_usuario", filtro.UsuarioId);
                p.Add("p_tipo", filtro.TipoMovimientoId);
                p.Add("p_fecha_desde", filtro.FechaDesde);
                p.Add("p_fecha_hasta", filtro.FechaHasta);
                p.Add("p_orden_venta", filtro.OrdenVentaId);
                p.Add("p_tipo_nombre", filtro.TipoMovimiento);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[INV REPOS] Consultando movimientos globales...");

                var results = await connection.QueryAsync<KardexDTO>(new CommandDefinition(
                    "PKG_INVENTARIO.sp_obtener_movimientos_globales", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[INV REPOS] OK: {Count} movimientos encontrados.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[INV REPOS] Error en ObtenerMovimientosGlobalesAsync: {Message}", ex.Message);
                throw;
            }
        }
    }
}
