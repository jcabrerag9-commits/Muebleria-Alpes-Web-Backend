using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class PrecioRepository : BaseRepository<PrecioRepository>, IPrecioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PrecioRepository(OracleConnectionFactory connectionFactory, ILogger<PrecioRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(PrecioProducto precio)
        {
            _logger.LogInformation("[AUDITORÍA-PRECIO] Iniciando CreateAsync para ProductoId: {Id}", precio.ProductoId);
            
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            
            // Auditoría exhaustiva de parámetros antes de enviar a Oracle
            _logger.LogInformation("[AUDITORÍA-PRECIO] Enviando a Oracle: Producto={P}, Variante={V}, Moneda={M}, Precio={PR}, Oferta={OF}, Inicio={FI}", 
                precio.ProductoId, precio.VarianteId, precio.MonedaId, precio.Precio, precio.PrecioOferta, precio.FechaInicio);

            parameters.Add("p_producto", precio.ProductoId, OracleDbType.Int32, ParameterDirection.Input);
            
            // SANEAMIENTO: Si es 0 o null, mandamos DBNull para evitar fallos de FK en Oracle
            object? v_variante = (precio.VarianteId == 0) ? null : precio.VarianteId;
            parameters.Add("p_variante", v_variante, OracleDbType.Int32, ParameterDirection.Input);
            
            // SANEAMIENTO: Garantizar moneda base si viene en 0
            int v_moneda = (precio.MonedaId == 0) ? 1 : precio.MonedaId;
            parameters.Add("p_moneda", v_moneda, OracleDbType.Int32, ParameterDirection.Input);
            
            parameters.Add("p_tipo", precio.Tipo ?? "REGULAR", OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_precio", precio.Precio, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_precio_oferta", precio.PrecioOferta, OracleDbType.Decimal, ParameterDirection.Input);
            
            // FECHA: Usar la fecha proporcionada o SYSDATE (Now)
            DateTime v_inicio = (precio.FechaInicio == DateTime.MinValue) ? DateTime.Now : precio.FechaInicio;
            parameters.Add("p_fecha_inicio", v_inicio, OracleDbType.Date, ParameterDirection.Input);
            
            parameters.Add("p_fecha_fin", precio.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_id_nuevo", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            try
            {
                _logger.LogInformation("[AUDITORÍA-PRECIO] Ejecutando SP: PKG_PRECIOS.sp_registrar_precio_producto");
                await connection.ExecuteAsync("PKG_PRECIOS.sp_registrar_precio_producto", parameters, commandType: CommandType.StoredProcedure);
                
                int idNuevo = parameters.Get<int>("p_id_nuevo");
                _logger.LogInformation("[AUDITORÍA-PRECIO] ÉXITO: Registro creado con ID {Id}", idNuevo);
                return idNuevo;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "[ERROR-ORACLE] Falló el INSERT en ALP_PRODUCTO_PRECIO. ORA-{Num}: {Msg}", ex.Number, ex.Message);
                _logger.LogCritical("[AUDITORÍA-CRÍTICA] ¿Falla por Constraint? Compruebe NOT NULLs o Foreign Keys.");
                throw;
            }
        }

        public async Task UpdateAsync(ActualizarPrecioRequest request)
        {
            _logger.LogInformation("[AUDITORÍA-PRECIO] Iniciando UpdateAsync para PrecioId: {Id}", request.PrecioId);
            
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();
            
            parameters.Add("p_precio_id", request.PrecioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_precio", request.NuevoPrecio, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_precio_oferta", request.NuevoPrecioOferta, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", request.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario", request.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_motivo", request.Motivo ?? "Actualización manual", OracleDbType.Varchar2, ParameterDirection.Input);

            try
            {
                await connection.ExecuteAsync("PKG_PRECIOS.sp_actualizar_precio_producto", parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("[AUDITORÍA-PRECIO] SP sp_actualizar_precio_producto ejecutado con éxito.");
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "[ERROR-ORACLE] Fallo en sp_actualizar_precio_producto. ORA-{Num}", ex.Number);
                throw;
            }
        }

        public async Task<decimal> GetPrecioVigenteAsync(int productoId, int monedaId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<decimal>(
                    "SELECT PKG_PRECIOS.fn_obtener_precio_vigente_producto(:productoId, :monedaId) FROM DUAL",
                    new { productoId, monedaId }
                );
            }
            catch
            {
                // Fallback: PKG_PRECIOS no está compilado — consulta directa a ALP_PRODUCTO_PRECIO
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    SELECT PPR_PRECIO FROM (
                        SELECT PPR_PRECIO FROM ALP_PRODUCTO_PRECIO
                        WHERE PRO_PRODUCTO = :productoId
                          AND MON_MONEDA   = :monedaId
                          AND PPR_ESTADO   = 'ACTIVO'
                          AND PPR_FECHA_INICIO <= SYSDATE
                          AND (PPR_FECHA_FIN IS NULL OR PPR_FECHA_FIN >= SYSDATE)
                        ORDER BY PPR_FECHA_INICIO DESC, PPR_PRODUCTO_PRECIO DESC
                    ) WHERE ROWNUM = 1";
                return await connection.ExecuteScalarAsync<decimal>(sql, new { productoId, monedaId });
            }
        }

        public async Task<decimal> GetPrecioFinalAsync(int productoId, int monedaId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<decimal>(
                    "SELECT PKG_PRECIOS.fn_calcular_precio_final_producto(:productoId, :monedaId) FROM DUAL",
                    new { productoId, monedaId }
                );
            }
            catch
            {
                // Fallback: misma lógica que GetPrecioVigenteAsync — precio base sin descuentos
                return await GetPrecioVigenteAsync(productoId, monedaId);
            }
        }

        public async Task<IEnumerable<PrecioProducto>> GetHistorialByProductoAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"
                SELECT PPR_PRODUCTO_PRECIO as Id, PRO_PRODUCTO as ProductoId, PVA_PRODUCTO_VARIANTE as VarianteId, 
                       MON_MONEDA as MonedaId, PPR_TIPO as Tipo, PPR_PRECIO as Precio, PPR_PRECIO_OFERTA as PrecioOferta, 
                       PPR_FECHA_INICIO as FechaInicio, PPR_FECHA_FIN as FechaFin, PPR_ESTADO as Estado 
                FROM ALP_PRODUCTO_PRECIO 
                WHERE PRO_PRODUCTO = :productoId 
                ORDER BY PPR_FECHA_INICIO DESC";
            return await connection.QueryAsync<PrecioProducto>(query, new { productoId });
        }

        public async Task<IEnumerable<PrecioProducto>> GetPreciosVigentesMasivoAsync(int monedaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Consulta optimizada para traer el último precio activo de cada producto en una sola pasada
            const string sql = @"
                SELECT PPR_PRODUCTO_PRECIO as Id, PRO_PRODUCTO as ProductoId, PPR_PRECIO as Precio, PPR_PRECIO_OFERTA as PrecioOferta
                FROM (
                    SELECT PPR_PRODUCTO_PRECIO, PRO_PRODUCTO, PPR_PRECIO, PPR_PRECIO_OFERTA,
                           ROW_NUMBER() OVER (PARTITION BY PRO_PRODUCTO ORDER BY PPR_FECHA_INICIO DESC, PPR_PRODUCTO_PRECIO DESC) as rn
                    FROM ALP_PRODUCTO_PRECIO
                    WHERE MON_MONEDA = :monedaId
                      AND PPR_ESTADO = 'ACTIVO'
                      AND PPR_FECHA_INICIO <= SYSDATE
                      AND (PPR_FECHA_FIN IS NULL OR PPR_FECHA_FIN >= SYSDATE)
                ) WHERE rn = 1";
            
            return await connection.QueryAsync<PrecioProducto>(sql, new { monedaId });
        }
    }
}
