using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class PromocionRepository : IPromocionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public PromocionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ── Consultas ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<Promocion>> GetAllAsync(string? estado = null, string? tipo = null)
        {
            var sql = @"SELECT PRM_PROMOCION      AS PrmPromocion,
                               PRM_CODIGO         AS PrmCodigo,
                               PRM_NOMBRE         AS PrmNombre,
                               PRM_DESCRIPCION    AS PrmDescripcion,
                               PRM_TIPO           AS PrmTipo,
                               PRM_VALOR          AS PrmValor,
                               PRM_FECHA_INICIO   AS PrmFechaInicio,
                               PRM_FECHA_FIN      AS PrmFechaFin,
                               PRM_ESTADO         AS PrmEstado
                        FROM   ALP_PROMOCION
                        WHERE  (:estado IS NULL OR PRM_ESTADO = :estado)
                          AND  (:tipo   IS NULL OR PRM_TIPO   = :tipo)
                        ORDER BY PRM_FECHA_INICIO DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Promocion>(sql, new
            {
                estado = estado?.ToUpper(),
                tipo   = tipo?.ToUpper()
            });
        }

        public async Task<Promocion?> GetByIdAsync(long id)
        {
            var sql = @"SELECT PRM_PROMOCION      AS PrmPromocion,
                               PRM_CODIGO         AS PrmCodigo,
                               PRM_NOMBRE         AS PrmNombre,
                               PRM_DESCRIPCION    AS PrmDescripcion,
                               PRM_TIPO           AS PrmTipo,
                               PRM_VALOR          AS PrmValor,
                               PRM_FECHA_INICIO   AS PrmFechaInicio,
                               PRM_FECHA_FIN      AS PrmFechaFin,
                               PRM_ESTADO         AS PrmEstado
                        FROM   ALP_PROMOCION
                        WHERE  PRM_PROMOCION = :id";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Promocion>(sql, new { id });
        }

        public async Task<Promocion?> GetByCodigoAsync(string codigo)
        {
            var sql = @"SELECT PRM_PROMOCION      AS PrmPromocion,
                               PRM_CODIGO         AS PrmCodigo,
                               PRM_NOMBRE         AS PrmNombre,
                               PRM_DESCRIPCION    AS PrmDescripcion,
                               PRM_TIPO           AS PrmTipo,
                               PRM_VALOR          AS PrmValor,
                               PRM_FECHA_INICIO   AS PrmFechaInicio,
                               PRM_FECHA_FIN      AS PrmFechaFin,
                               PRM_ESTADO         AS PrmEstado
                        FROM   ALP_PROMOCION
                        WHERE  PRM_CODIGO = :codigo";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Promocion>(sql, new { codigo = codigo.ToUpper() });
        }

        public async Task<IEnumerable<Promocion>> GetVigentesAsync()
        {
            var sql = @"SELECT PRM_PROMOCION      AS PrmPromocion,
                               PRM_CODIGO         AS PrmCodigo,
                               PRM_NOMBRE         AS PrmNombre,
                               PRM_DESCRIPCION    AS PrmDescripcion,
                               PRM_TIPO           AS PrmTipo,
                               PRM_VALOR          AS PrmValor,
                               PRM_FECHA_INICIO   AS PrmFechaInicio,
                               PRM_FECHA_FIN      AS PrmFechaFin,
                               PRM_ESTADO         AS PrmEstado
                        FROM   ALP_PROMOCION
                        WHERE  PRM_ESTADO = 'ACTIVO'
                          AND  PRM_FECHA_INICIO <= SYSDATE
                          AND  PRM_FECHA_FIN    >= SYSDATE
                        ORDER BY PRM_FECHA_FIN";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Promocion>(sql);
        }

        // ── CRUD ──────────────────────────────────────────────────────────────

        public async Task<long> CreateAsync(Promocion p)
        {
            var sql = @"INSERT INTO ALP_PROMOCION
                            (PRM_CODIGO, PRM_NOMBRE, PRM_DESCRIPCION, PRM_TIPO,
                             PRM_VALOR, PRM_FECHA_INICIO, PRM_FECHA_FIN, PRM_ESTADO)
                        VALUES
                            (:PrmCodigo, :PrmNombre, :PrmDescripcion, :PrmTipo,
                             :PrmValor, :PrmFechaInicio, :PrmFechaFin, :PrmEstado)
                        RETURNING PRM_PROMOCION INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var parametros = new DynamicParameters();
            parametros.Add("PrmCodigo",      p.PrmCodigo);
            parametros.Add("PrmNombre",      p.PrmNombre);
            parametros.Add("PrmDescripcion", p.PrmDescripcion);
            parametros.Add("PrmTipo",        p.PrmTipo);
            parametros.Add("PrmValor",       p.PrmValor);
            parametros.Add("PrmFechaInicio", p.PrmFechaInicio);
            parametros.Add("PrmFechaFin",    p.PrmFechaFin);
            parametros.Add("PrmEstado",      p.PrmEstado);
            parametros.Add("newId",          dbType: System.Data.DbType.Int64,
                           direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, parametros);
            return parametros.Get<long>("newId");
        }

        public async Task<bool> UpdateAsync(Promocion p)
        {
            var sql = @"UPDATE ALP_PROMOCION
                        SET PRM_NOMBRE       = :PrmNombre,
                            PRM_DESCRIPCION  = :PrmDescripcion,
                            PRM_VALOR        = :PrmValor,
                            PRM_FECHA_INICIO = :PrmFechaInicio,
                            PRM_FECHA_FIN    = :PrmFechaFin,
                            PRM_ESTADO       = :PrmEstado
                        WHERE PRM_PROMOCION  = :PrmPromocion";

            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, p);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var sql = "DELETE FROM ALP_PROMOCION WHERE PRM_PROMOCION = :id";
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            var sql = "SELECT COUNT(1) FROM ALP_PROMOCION WHERE PRM_PROMOCION = :id";
            using var conn = _connectionFactory.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        }

        public async Task<bool> CodigoExistsAsync(string codigo, long? excludeId = null)
        {
            var sql = @"SELECT COUNT(1) FROM ALP_PROMOCION
                        WHERE PRM_CODIGO = :codigo
                          AND (:excludeId IS NULL OR PRM_PROMOCION <> :excludeId)";
            using var conn = _connectionFactory.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { codigo = codigo.ToUpper(), excludeId });
            return count > 0;
        }

        // ── PromocionProducto ─────────────────────────────────────────────────

        public async Task<IEnumerable<PromocionProducto>> GetProductosByPromocionAsync(long promocionId)
        {
            var sql = @"SELECT PPO_PROMOCION_PRODUCTO   AS PpoPromocionProducto,
                               PRM_PROMOCION            AS PrmPromocion,
                               PRO_PRODUCTO             AS ProProducto,
                               PVA_PRODUCTO_VARIANTE    AS PvaProductoVariante,
                               PPO_ESTADO               AS PpoEstado,
                               PPO_FECHA_CREACION       AS PpoFechaCreacion
                        FROM   ALP_PROMOCION_PRODUCTO
                        WHERE  PRM_PROMOCION = :promocionId
                          AND  PPO_ESTADO = 'ACTIVO'";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<PromocionProducto>(sql, new { promocionId });
        }

        public async Task<long> AddProductoAsync(PromocionProducto pp)
        {
            var sql = @"INSERT INTO ALP_PROMOCION_PRODUCTO
                            (PRM_PROMOCION, PRO_PRODUCTO, PVA_PRODUCTO_VARIANTE, PPO_ESTADO, PPO_FECHA_CREACION)
                        VALUES
                            (:PrmPromocion, :ProProducto, :PvaProductoVariante, :PpoEstado, :PpoFechaCreacion)
                        RETURNING PPO_PROMOCION_PRODUCTO INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var parametros = new DynamicParameters();
            parametros.Add("PrmPromocion",        pp.PrmPromocion);
            parametros.Add("ProProducto",         pp.ProProducto);
            parametros.Add("PvaProductoVariante", pp.PvaProductoVariante);
            parametros.Add("PpoEstado",           pp.PpoEstado);
            parametros.Add("PpoFechaCreacion",    pp.PpoFechaCreacion);
            parametros.Add("newId",               dbType: System.Data.DbType.Int64,
                           direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, parametros);
            return parametros.Get<long>("newId");
        }

        public async Task<bool> RemoveProductoAsync(long ppoId)
        {
            var sql = "DELETE FROM ALP_PROMOCION_PRODUCTO WHERE PPO_PROMOCION_PRODUCTO = :ppoId";
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { ppoId });
            return rows > 0;
        }
    }
}
