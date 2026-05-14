using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public BannerRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ── Banners ───────────────────────────────────────────────────────────

        public async Task<IEnumerable<Banner>> GetAllAsync(string? estado = null, string? posicion = null)
        {
            var sql = @"SELECT BAN_BANNER       AS BanBanner,
                               BAN_TITULO       AS BanTitulo,
                               BAN_IMAGEN_URL   AS BanImagenUrl,
                               BAN_ENLACE       AS BanEnlace,
                               BAN_POSICION     AS BanPosicion,
                               BAN_FECHA_INICIO AS BanFechaInicio,
                               BAN_FECHA_FIN    AS BanFechaFin,
                               BAN_ESTADO       AS BanEstado
                        FROM   ALP_BANNER
                        WHERE  (:estado   IS NULL OR BAN_ESTADO   = :estado)
                          AND  (:posicion IS NULL OR BAN_POSICION = :posicion)
                        ORDER BY BAN_FECHA_INICIO DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Banner>(sql, new
            {
                estado   = estado?.ToUpper(),
                posicion = posicion?.ToUpper()
            });
        }

        public async Task<Banner?> GetByIdAsync(long id)
        {
            var sql = @"SELECT BAN_BANNER       AS BanBanner,
                               BAN_TITULO       AS BanTitulo,
                               BAN_IMAGEN_URL   AS BanImagenUrl,
                               BAN_ENLACE       AS BanEnlace,
                               BAN_POSICION     AS BanPosicion,
                               BAN_FECHA_INICIO AS BanFechaInicio,
                               BAN_FECHA_FIN    AS BanFechaFin,
                               BAN_ESTADO       AS BanEstado
                        FROM   ALP_BANNER
                        WHERE  BAN_BANNER = :id";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Banner>(sql, new { id });
        }

        public async Task<IEnumerable<Banner>> GetVigentesAsync(string? posicion = null)
        {
            var sql = @"SELECT BAN_BANNER       AS BanBanner,
                               BAN_TITULO       AS BanTitulo,
                               BAN_IMAGEN_URL   AS BanImagenUrl,
                               BAN_ENLACE       AS BanEnlace,
                               BAN_POSICION     AS BanPosicion,
                               BAN_FECHA_INICIO AS BanFechaInicio,
                               BAN_FECHA_FIN    AS BanFechaFin,
                               BAN_ESTADO       AS BanEstado
                        FROM   ALP_BANNER
                        WHERE  BAN_ESTADO = 'ACTIVO'
                          AND  BAN_FECHA_INICIO <= SYSDATE
                          AND  (BAN_FECHA_FIN IS NULL OR BAN_FECHA_FIN >= SYSDATE)
                          AND  (:posicion IS NULL OR BAN_POSICION = :posicion)
                        ORDER BY BAN_FECHA_INICIO";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Banner>(sql, new { posicion = posicion?.ToUpper() });
        }

        public async Task<long> CreateAsync(Banner b)
        {
            var sql = @"INSERT INTO ALP_BANNER
                            (BAN_TITULO, BAN_IMAGEN_URL, BAN_ENLACE,
                             BAN_POSICION, BAN_FECHA_INICIO, BAN_FECHA_FIN, BAN_ESTADO)
                        VALUES
                            (:BanTitulo, :BanImagenUrl, :BanEnlace,
                             :BanPosicion, :BanFechaInicio, :BanFechaFin, :BanEstado)
                        RETURNING BAN_BANNER INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var p = new DynamicParameters();
            p.Add("BanTitulo",      b.BanTitulo);
            p.Add("BanImagenUrl",   b.BanImagenUrl);
            p.Add("BanEnlace",      b.BanEnlace);
            p.Add("BanPosicion",    b.BanPosicion);
            p.Add("BanFechaInicio", b.BanFechaInicio);
            p.Add("BanFechaFin",    b.BanFechaFin);
            p.Add("BanEstado",      b.BanEstado);
            p.Add("newId",          dbType: System.Data.DbType.Int64,
                  direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p);
            return p.Get<long>("newId");
        }

        public async Task<bool> UpdateAsync(Banner b)
        {
            var sql = @"UPDATE ALP_BANNER
                        SET BAN_TITULO       = :BanTitulo,
                            BAN_IMAGEN_URL   = :BanImagenUrl,
                            BAN_ENLACE       = :BanEnlace,
                            BAN_POSICION     = :BanPosicion,
                            BAN_FECHA_INICIO = :BanFechaInicio,
                            BAN_FECHA_FIN    = :BanFechaFin,
                            BAN_ESTADO       = :BanEstado
                        WHERE BAN_BANNER = :BanBanner";

            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, b);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var sql = "DELETE FROM ALP_BANNER WHERE BAN_BANNER = :id";
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            var sql = "SELECT COUNT(1) FROM ALP_BANNER WHERE BAN_BANNER = :id";
            using var conn = _connectionFactory.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        }

        // ── Clicks ────────────────────────────────────────────────────────────

        public async Task<long> RegistrarClickAsync(ClickBanner c)
        {
            var sql = @"INSERT INTO ALP_CLICK_BANNER
                            (BAN_BANNER, CLI_CLIENTE, CLB_FECHA_CLICK,
                             CLB_PLATAFORMA, CLB_ORIGEN, CLB_DETALLE)
                        VALUES
                            (:BanBanner, :CliCliente, :ClbFechaClick,
                             :ClbPlataforma, :ClbOrigen, :ClbDetalle)
                        RETURNING CLB_CLICK_BANNER INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var p = new DynamicParameters();
            p.Add("BanBanner",     c.BanBanner);
            p.Add("CliCliente",    c.CliCliente);
            p.Add("ClbFechaClick", c.ClbFechaClick);
            p.Add("ClbPlataforma", c.ClbPlataforma);
            p.Add("ClbOrigen",     c.ClbOrigen);
            p.Add("ClbDetalle",    c.ClbDetalle);
            p.Add("newId",         dbType: System.Data.DbType.Int64,
                  direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p);
            return p.Get<long>("newId");
        }

        public async Task<IEnumerable<ClickBanner>> GetClicksByBannerAsync(long bannerId)
        {
            var sql = @"SELECT CLB_CLICK_BANNER AS ClbClickBanner,
                               BAN_BANNER       AS BanBanner,
                               CLI_CLIENTE      AS CliCliente,
                               CLB_FECHA_CLICK  AS ClbFechaClick,
                               CLB_PLATAFORMA   AS ClbPlataforma,
                               CLB_ORIGEN       AS ClbOrigen,
                               CLB_DETALLE      AS ClbDetalle
                        FROM   ALP_CLICK_BANNER
                        WHERE  BAN_BANNER = :bannerId
                        ORDER BY CLB_FECHA_CLICK DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<ClickBanner>(sql, new { bannerId });
        }

        public async Task<int> GetTotalClicksAsync(long bannerId)
        {
            var sql = "SELECT COUNT(1) FROM ALP_CLICK_BANNER WHERE BAN_BANNER = :bannerId";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { bannerId });
        }
    }
}