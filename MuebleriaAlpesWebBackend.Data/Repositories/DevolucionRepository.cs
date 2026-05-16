using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Entities;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class DevolucionRepository : IDevolucionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public DevolucionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ════════════════════════════════════════════════════════════════════
        // CATEGORÍAS — CRUD COMPLETO
        // ════════════════════════════════════════════════════════════════════

        public async Task<IEnumerable<CategoriaDevolucion>> GetCategoriasAsync(string? estado = null)
        {
            var sql = @"SELECT CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               CTD_CODIGO             AS CtdCodigo,
                               CTD_NOMBRE             AS CtdNombre,
                               CTD_DESCRIPCION        AS CtdDescripcion,
                               CTD_ESTADO             AS CtdEstado
                        FROM   ALP_CATEGORIA_TIPO_DEVOLUCION
                        WHERE  (:estado IS NULL OR CTD_ESTADO = :estado)
                        ORDER BY CTD_NOMBRE";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<CategoriaDevolucion>(sql, new { estado = estado?.ToUpper() });
        }

        public async Task<CategoriaDevolucion?> GetCategoriaByIdAsync(long id)
        {
            var sql = @"SELECT CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               CTD_CODIGO             AS CtdCodigo,
                               CTD_NOMBRE             AS CtdNombre,
                               CTD_DESCRIPCION        AS CtdDescripcion,
                               CTD_ESTADO             AS CtdEstado
                        FROM   ALP_CATEGORIA_TIPO_DEVOLUCION
                        WHERE  CTD_CATEGORIA_TIPO_DEV = :id";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CategoriaDevolucion>(sql, new { id });
        }

        public async Task<CategoriaDevolucion?> GetCategoriaByCodigoAsync(string codigo)
        {
            var sql = @"SELECT CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               CTD_CODIGO             AS CtdCodigo,
                               CTD_NOMBRE             AS CtdNombre,
                               CTD_DESCRIPCION        AS CtdDescripcion,
                               CTD_ESTADO             AS CtdEstado
                        FROM   ALP_CATEGORIA_TIPO_DEVOLUCION
                        WHERE  CTD_CODIGO = :codigo";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CategoriaDevolucion>(sql, new { codigo = codigo.ToUpper() });
        }

        public async Task<long> CreateCategoriaAsync(CategoriaDevolucion c)
        {
            // CTD_CODIGO se omite del INSERT — el trigger TRG_CTD_CODIGO lo genera automáticamente
            var sql = @"INSERT INTO ALP_CATEGORIA_TIPO_DEVOLUCION
                            (CTD_NOMBRE, CTD_DESCRIPCION, CTD_ESTADO)
                        VALUES
                            (:CtdNombre, :CtdDescripcion, :CtdEstado)
                        RETURNING CTD_CATEGORIA_TIPO_DEV INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var p = new DynamicParameters();
            p.Add("CtdNombre",      c.CtdNombre);
            p.Add("CtdDescripcion", c.CtdDescripcion);
            p.Add("CtdEstado",      c.CtdEstado);
            p.Add("newId",          dbType: System.Data.DbType.Int64,
                  direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p);
            return p.Get<long>("newId");
        }

        public async Task<bool> UpdateCategoriaAsync(CategoriaDevolucion c)
        {
            // CTD_CODIGO nunca se actualiza — es generado y fijo
            var sql = @"UPDATE ALP_CATEGORIA_TIPO_DEVOLUCION
                        SET CTD_NOMBRE      = :CtdNombre,
                            CTD_DESCRIPCION = :CtdDescripcion,
                            CTD_ESTADO      = :CtdEstado
                        WHERE CTD_CATEGORIA_TIPO_DEV = :CtdCategoriaTipoDev";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, c) > 0;
        }

        public async Task<bool> DeleteCategoriaAsync(long id)
        {
            var sql = "DELETE FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = :id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, new { id }) > 0;
        }

        public async Task<bool> CategoriaExistsAsync(long id)
        {
            var sql = "SELECT COUNT(1) FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = :id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { id }) > 0;
        }

        public async Task<bool> CodigoCategoriaExistsAsync(string codigo, long? excludeId = null)
        {
            var sql = @"SELECT COUNT(1) FROM ALP_CATEGORIA_TIPO_DEVOLUCION
                        WHERE CTD_CODIGO = :codigo
                          AND (:excludeId IS NULL OR CTD_CATEGORIA_TIPO_DEV <> :excludeId)";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { codigo = codigo.ToUpper(), excludeId }) > 0;
        }

        public async Task<bool> CategoriaEnUsoAsync(long id)
        {
            var sql = "SELECT COUNT(1) FROM ALP_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = :id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { id }) > 0;
        }

        // ════════════════════════════════════════════════════════════════════
        // DEVOLUCIONES
        // ════════════════════════════════════════════════════════════════════

        public async Task<IEnumerable<Devolucion>> GetAllAsync(string? estado = null, long? clienteId = null)
        {
            var sql = @"SELECT d.DEV_DEVOLUCION         AS DevDevolucion,
                               d.VEN_ORDEN_VENTA        AS VenOrdenVenta,
                               d.CLI_CLIENTE            AS CliCliente,
                               d.CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               d.DEV_NUMERO_RMA         AS DevNumeroRma,
                               d.DEV_MOTIVO             AS DevMotivo,
                               d.DEV_MONTO_TOTAL        AS DevMontoTotal,
                               d.DEV_ESTADO             AS DevEstado,
                               d.DEV_FECHA_CREACION     AS DevFechaCreacion,
                               c.CTD_NOMBRE             AS NombreCategoria
                        FROM   ALP_DEVOLUCION d
                        JOIN   ALP_CATEGORIA_TIPO_DEVOLUCION c
                               ON c.CTD_CATEGORIA_TIPO_DEV = d.CTD_CATEGORIA_TIPO_DEV
                        WHERE  (:estado    IS NULL OR d.DEV_ESTADO  = :estado)
                          AND  (:clienteId IS NULL OR d.CLI_CLIENTE = :clienteId)
                        ORDER BY d.DEV_FECHA_CREACION DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Devolucion>(sql, new { estado = estado?.ToUpper(), clienteId });
        }

        public async Task<Devolucion?> GetByIdAsync(long id)
        {
            var sql = @"SELECT d.DEV_DEVOLUCION         AS DevDevolucion,
                               d.VEN_ORDEN_VENTA        AS VenOrdenVenta,
                               d.CLI_CLIENTE            AS CliCliente,
                               d.CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               d.DEV_NUMERO_RMA         AS DevNumeroRma,
                               d.DEV_MOTIVO             AS DevMotivo,
                               d.DEV_MONTO_TOTAL        AS DevMontoTotal,
                               d.DEV_ESTADO             AS DevEstado,
                               d.DEV_FECHA_CREACION     AS DevFechaCreacion,
                               c.CTD_NOMBRE             AS NombreCategoria
                        FROM   ALP_DEVOLUCION d
                        JOIN   ALP_CATEGORIA_TIPO_DEVOLUCION c
                               ON c.CTD_CATEGORIA_TIPO_DEV = d.CTD_CATEGORIA_TIPO_DEV
                        WHERE  d.DEV_DEVOLUCION = :id";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Devolucion>(sql, new { id });
        }

        public async Task<Devolucion?> GetByRmaAsync(string numeroRma)
        {
            var sql = @"SELECT d.DEV_DEVOLUCION         AS DevDevolucion,
                               d.VEN_ORDEN_VENTA        AS VenOrdenVenta,
                               d.CLI_CLIENTE            AS CliCliente,
                               d.CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               d.DEV_NUMERO_RMA         AS DevNumeroRma,
                               d.DEV_MOTIVO             AS DevMotivo,
                               d.DEV_MONTO_TOTAL        AS DevMontoTotal,
                               d.DEV_ESTADO             AS DevEstado,
                               d.DEV_FECHA_CREACION     AS DevFechaCreacion,
                               c.CTD_NOMBRE             AS NombreCategoria
                        FROM   ALP_DEVOLUCION d
                        JOIN   ALP_CATEGORIA_TIPO_DEVOLUCION c
                               ON c.CTD_CATEGORIA_TIPO_DEV = d.CTD_CATEGORIA_TIPO_DEV
                        WHERE  d.DEV_NUMERO_RMA = :numeroRma";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Devolucion>(sql, new { numeroRma = numeroRma.ToUpper() });
        }

        public async Task<IEnumerable<Devolucion>> GetByOrdenVentaAsync(long ordenVentaId)
        {
            var sql = @"SELECT d.DEV_DEVOLUCION         AS DevDevolucion,
                               d.VEN_ORDEN_VENTA        AS VenOrdenVenta,
                               d.CLI_CLIENTE            AS CliCliente,
                               d.CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               d.DEV_NUMERO_RMA         AS DevNumeroRma,
                               d.DEV_MOTIVO             AS DevMotivo,
                               d.DEV_MONTO_TOTAL        AS DevMontoTotal,
                               d.DEV_ESTADO             AS DevEstado,
                               d.DEV_FECHA_CREACION     AS DevFechaCreacion,
                               c.CTD_NOMBRE             AS NombreCategoria
                        FROM   ALP_DEVOLUCION d
                        JOIN   ALP_CATEGORIA_TIPO_DEVOLUCION c
                               ON c.CTD_CATEGORIA_TIPO_DEV = d.CTD_CATEGORIA_TIPO_DEV
                        WHERE  d.VEN_ORDEN_VENTA = :ordenVentaId";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Devolucion>(sql, new { ordenVentaId });
        }

        public async Task<IEnumerable<Devolucion>> GetByClienteAsync(long clienteId)
        {
            var sql = @"SELECT d.DEV_DEVOLUCION         AS DevDevolucion,
                               d.VEN_ORDEN_VENTA        AS VenOrdenVenta,
                               d.CLI_CLIENTE            AS CliCliente,
                               d.CTD_CATEGORIA_TIPO_DEV AS CtdCategoriaTipoDev,
                               d.DEV_NUMERO_RMA         AS DevNumeroRma,
                               d.DEV_MOTIVO             AS DevMotivo,
                               d.DEV_MONTO_TOTAL        AS DevMontoTotal,
                               d.DEV_ESTADO             AS DevEstado,
                               d.DEV_FECHA_CREACION     AS DevFechaCreacion,
                               c.CTD_NOMBRE             AS NombreCategoria
                        FROM   ALP_DEVOLUCION d
                        JOIN   ALP_CATEGORIA_TIPO_DEVOLUCION c
                               ON c.CTD_CATEGORIA_TIPO_DEV = d.CTD_CATEGORIA_TIPO_DEV
                        WHERE  d.CLI_CLIENTE = :clienteId
                        ORDER BY d.DEV_FECHA_CREACION DESC";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<Devolucion>(sql, new { clienteId });
        }

        public async Task<long> CreateAsync(Devolucion d)
        {
            var sql = @"INSERT INTO ALP_DEVOLUCION
                            (VEN_ORDEN_VENTA, CLI_CLIENTE, CTD_CATEGORIA_TIPO_DEV,
                             DEV_NUMERO_RMA, DEV_MOTIVO, DEV_MONTO_TOTAL,
                             DEV_ESTADO, DEV_FECHA_CREACION)
                        VALUES
                            (:VenOrdenVenta, :CliCliente, :CtdCategoriaTipoDev,
                             :DevNumeroRma, :DevMotivo, :DevMontoTotal,
                             :DevEstado, :DevFechaCreacion)
                        RETURNING DEV_DEVOLUCION INTO :newId";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();

            var p = new DynamicParameters();
            p.Add("VenOrdenVenta",       d.VenOrdenVenta);
            p.Add("CliCliente",          d.CliCliente);
            p.Add("CtdCategoriaTipoDev", d.CtdCategoriaTipoDev);
            p.Add("DevNumeroRma",        d.DevNumeroRma);
            p.Add("DevMotivo",           d.DevMotivo);
            p.Add("DevMontoTotal",       d.DevMontoTotal);
            p.Add("DevEstado",           d.DevEstado);
            p.Add("DevFechaCreacion",    d.DevFechaCreacion);
            p.Add("newId",               dbType: System.Data.DbType.Int64,
                  direction: System.Data.ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p);
            return p.Get<long>("newId");
        }

        public async Task<bool> UpdateEstadoAsync(long id, string nuevoEstado)
        {
            var sql = @"UPDATE ALP_DEVOLUCION SET DEV_ESTADO = :nuevoEstado
                        WHERE DEV_DEVOLUCION = :id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, new { nuevoEstado, id }) > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var sql = "DELETE FROM ALP_DEVOLUCION WHERE DEV_DEVOLUCION = :id AND DEV_ESTADO = 'SOLICITADA'";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, new { id }) > 0;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            var sql = "SELECT COUNT(1) FROM ALP_DEVOLUCION WHERE DEV_DEVOLUCION = :id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { id }) > 0;
        }

        public async Task<string> GenerarNumeroRmaAsync()
        {
            var sql = @"SELECT COUNT(1) FROM ALP_DEVOLUCION
                        WHERE EXTRACT(YEAR FROM DEV_FECHA_CREACION) = EXTRACT(YEAR FROM SYSDATE)";
            using var conn = _connectionFactory.CreateConnection();
            var total = await conn.ExecuteScalarAsync<int>(sql);
            return $"RMA-{DateTime.Now.Year}-{(total + 1):D8}";
        }

        // ════════════════════════════════════════════════════════════════════
        // DETALLES
        // ════════════════════════════════════════════════════════════════════

        public async Task<IEnumerable<DevolucionDetalle>> GetDetallesByDevolucionAsync(long devolucionId)
        {
            var sql = @"SELECT DDE_DEVOLUCION_DETALLE  AS DdeDevolucionDetalle,
                               DEV_DEVOLUCION          AS DevDevolucion,
                               VDE_ORDEN_VENTA_DETALLE AS VdeOrdenVentaDetalle,
                               DDE_CANTIDAD            AS DdeCantidad,
                               DDE_MONTO               AS DdeMonto,
                               DDE_ESTADO              AS DdeEstado,
                               DDE_FECHA_CREACION      AS DdeFechaCreacion
                        FROM   ALP_DEVOLUCION_DETALLE
                        WHERE  DEV_DEVOLUCION = :devolucionId";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<DevolucionDetalle>(sql, new { devolucionId });
        }

        public async Task AddDetallesAsync(IEnumerable<DevolucionDetalle> detalles)
        {
            var sql = @"INSERT INTO ALP_DEVOLUCION_DETALLE
                            (DEV_DEVOLUCION, VDE_ORDEN_VENTA_DETALLE,
                             DDE_CANTIDAD, DDE_MONTO, DDE_ESTADO, DDE_FECHA_CREACION)
                        VALUES
                            (:DevDevolucion, :VdeOrdenVentaDetalle,
                             :DdeCantidad, :DdeMonto, :DdeEstado, :DdeFechaCreacion)";

            using var conn = _connectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, detalles);
        }
    }
}