using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class BodegaRepository : BaseRepository<BodegaRepository>, IBodegaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public BodegaRepository(OracleConnectionFactory connectionFactory, ILogger<BodegaRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> CrearBodegaAsync(BodegaDTO bodega, int usuarioId, CancellationToken ct = default)
        {
            ValidarParametroNulo(bodega, nameof(bodega));
            LogOperacionInicio(nameof(CrearBodegaAsync), new { bodega.Nombre, usuarioId });

            var p = new OracleDynamicParameters();
            p.Add("p_nombre", bodega.Nombre);
            p.Add("p_descripcion", bodega.Descripcion);
            p.Add("p_ubicacion", bodega.Ubicacion);
            p.Add("p_tipo", bodega.Tipo);
            p.Add("p_canal_venta", bodega.CanalVentaId);
            p.Add("p_permite_reserva", bodega.PermiteReserva);
            p.Add("p_permite_venta", bodega.PermiteVenta);
            p.Add("p_maneja_despacho", bodega.ManejaDespacho);
            p.Add("p_usuario", usuarioId);
            p.Add("p_id_nuevo", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[BODEGA] SP=PKG_BODEGAS.sp_crear_bodega Nombre={Nombre} Tipo={Tipo} Canal={Canal}", bodega.Nombre, bodega.Tipo, bodega.CanalVentaId);
                await connection.ExecuteAsync(new CommandDefinition("PKG_BODEGAS.sp_crear_bodega", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                int? id = p.Get<int?>("p_id_nuevo");
                
                LogOperacionExito(nameof(CrearBodegaAsync), id);
                return new InventarioResponse<int?> { Resultado = "EXITO", Mensaje = "Bodega creada con éxito", Data = id };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<int?>(ex);
            }
        }

        public async Task<InventarioResponse<bool>> ActualizarBodegaAsync(BodegaDTO bodega, int usuarioId, CancellationToken ct = default)
        {
            ValidarParametroNulo(bodega, nameof(bodega));
            LogOperacionInicio(nameof(ActualizarBodegaAsync), new { bodega.BodegaId, usuarioId });

            var p = new OracleDynamicParameters();
            p.Add("p_bodega", bodega.BodegaId);
            p.Add("p_nombre", bodega.Nombre);
            p.Add("p_descripcion", bodega.Descripcion);
            p.Add("p_ubicacion", bodega.Ubicacion);
            p.Add("p_tipo", bodega.Tipo);
            p.Add("p_canal_venta", bodega.CanalVentaId);
            p.Add("p_permite_reserva", bodega.PermiteReserva);
            p.Add("p_permite_venta", bodega.PermiteVenta);
            p.Add("p_maneja_despacho", bodega.ManejaDespacho);
            p.Add("p_usuario", usuarioId);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[BODEGA] SP=PKG_BODEGAS.sp_actualizar_bodega ID={Id} Nombre={Nombre}", bodega.BodegaId, bodega.Nombre);
                await connection.ExecuteAsync(new CommandDefinition("PKG_BODEGAS.sp_actualizar_bodega", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                LogOperacionExito(nameof(ActualizarBodegaAsync));
                return new InventarioResponse<bool> { Resultado = "EXITO", Mensaje = "Bodega actualizada con éxito", Data = true };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<bool>(ex, false);
            }
        }

        public async Task<InventarioResponse<bool>> CambiarEstadoBodegaAsync(int bodegaId, string nuevoEstado, string motivo, int usuarioId, CancellationToken ct = default)
        {
            LogOperacionInicio(nameof(CambiarEstadoBodegaAsync), new { bodegaId, nuevoEstado, usuarioId });

            var p = new OracleDynamicParameters();
            p.Add("p_bodega", bodegaId);
            p.Add("p_estado", nuevoEstado);
            p.Add("p_motivo", motivo);
            p.Add("p_usuario", usuarioId);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[BODEGA] SP=PKG_BODEGAS.sp_cambiar_estado_bodega ID={Id} Estado={Estado}", bodegaId, nuevoEstado);
                await connection.ExecuteAsync(new CommandDefinition("PKG_BODEGAS.sp_cambiar_estado_bodega", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                
                LogOperacionExito(nameof(CambiarEstadoBodegaAsync));
                return new InventarioResponse<bool> { Resultado = "EXITO", Mensaje = "Estado de bodega actualizado", Data = true };
            }
            catch (OracleException ex)
            {
                return MapearErrorOracle<bool>(ex, false);
            }
        }

        public async Task<BodegaDTO?> ObtenerBodegaPorIdAsync(int bodegaId, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_bodega_id", bodegaId);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[BODEGA REPOS] Obteniendo bodega {BodegaId}...", bodegaId);

                var bodega = await connection.QueryFirstOrDefaultAsync<BodegaDTO>(new CommandDefinition(
                    "PKG_BODEGAS.sp_obtener_bodega_por_id", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                if (bodega != null)
                    _logger.LogInformation("[BODEGA REPOS] Bodega {BodegaId} encontrada: {Nombre}", bodegaId, bodega.Nombre);
                else
                    _logger.LogWarning("[BODEGA REPOS] Bodega {BodegaId} NO encontrada.", bodegaId);

                return bodega;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BODEGA REPOS] Error en ObtenerBodegaPorIdAsync: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<BodegaDTO>> ListarBodegasAsync(bool soloActivas = false, CancellationToken ct = default)
        {
            try 
            {
                var p = new OracleDynamicParameters();
                p.Add("p_solo_activas", soloActivas ? 1 : 0);
                p.Add("p_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

                using var connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("[BODEGA REPOS] Listando bodegas (Solo activas: {SoloActivas})...", soloActivas);

                var results = await connection.QueryAsync<BodegaDTO>(new CommandDefinition(
                    "PKG_BODEGAS.sp_listar_bodegas", 
                    p, 
                    commandType: CommandType.StoredProcedure, 
                    cancellationToken: ct
                ));

                var list = results.ToList();
                _logger.LogInformation("[BODEGA REPOS] OK: {Count} bodegas encontradas.", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BODEGA REPOS] Error en ListarBodegasAsync: {Message}", ex.Message);
                throw;
            }
        }
    }
}
