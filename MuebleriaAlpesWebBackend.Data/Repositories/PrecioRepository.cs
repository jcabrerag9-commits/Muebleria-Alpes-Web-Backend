using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class PrecioRepository : IPrecioRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PrecioRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(PrecioProducto precio)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_producto", precio.ProductoId);
            parameters.Add("p_variante", precio.VarianteId);
            parameters.Add("p_moneda", precio.MonedaId);
            parameters.Add("p_tipo", precio.Tipo);
            parameters.Add("p_precio", precio.Precio);
            parameters.Add("p_precio_oferta", precio.PrecioOferta);
            parameters.Add("p_fecha_inicio", precio.FechaInicio);
            parameters.Add("p_fecha_fin", precio.FechaFin);
            parameters.Add("p_id_nuevo", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_PRECIOS.sp_registrar_precio_producto", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_id_nuevo");
        }

        public async Task UpdateAsync(ActualizarPrecioRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_precio_id", request.PrecioId);
            parameters.Add("p_precio", request.NuevoPrecio);
            parameters.Add("p_precio_oferta", request.NuevoPrecioOferta);
            parameters.Add("p_fecha_fin", request.FechaFin);
            parameters.Add("p_usuario", request.UsuarioId);
            parameters.Add("p_motivo", request.Motivo);

            await connection.ExecuteAsync("PKG_PRECIOS.sp_actualizar_precio_producto", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<decimal> GetPrecioVigenteAsync(int productoId, int monedaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT PKG_PRECIOS.fn_obtener_precio_vigente_producto(:productoId, :monedaId) FROM DUAL",
                new { productoId, monedaId }
            );
        }

        public async Task<decimal> GetPrecioFinalAsync(int productoId, int monedaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT PKG_PRECIOS.fn_calcular_precio_final_producto(:productoId, :monedaId) FROM DUAL",
                new { productoId, monedaId }
            );
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
    }
}
