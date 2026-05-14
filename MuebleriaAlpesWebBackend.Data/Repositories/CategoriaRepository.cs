using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public CategoriaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// ALP_CATEGORIA: CAT_CATEGORIA, CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION
        /// No tiene columna de estado — se devuelven todas.
        /// </summary>
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT CAT_CATEGORIA  AS Id,
                       CAT_CODIGO     AS Codigo,
                       CAT_NOMBRE     AS Nombre,
                       CAT_DESCRIPCION AS Descripcion
                FROM   ALP_CATEGORIA
                ORDER BY CAT_NOMBRE";
            return await connection.QueryAsync<Categoria>(sql);
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT CAT_CATEGORIA  AS Id,
                       CAT_CODIGO     AS Codigo,
                       CAT_NOMBRE     AS Nombre,
                       CAT_DESCRIPCION AS Descripcion
                FROM   ALP_CATEGORIA
                WHERE  CAT_CATEGORIA = :id";
            return await connection.QueryFirstOrDefaultAsync<Categoria>(sql, new { id });
        }

        /// <summary>
        /// ALP_PRODUCTO_CATEGORIA: PCT_PRODUCTO_CATEGORIA (PK), PRO_PRODUCTO, CAT_CATEGORIA, PCT_PRINCIPAL
        /// No tiene columna de estado — sin filtro de estado.
        /// </summary>
        public async Task<IEnumerable<ProductoEnCategoria>> GetProductosByCategoriaAsync(int categoriaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT p.PRO_PRODUCTO  AS ProductoId,
                       p.PRO_NOMBRE    AS Nombre,
                       p.PRO_SKU       AS Sku,
                       p.PRO_ESTADO    AS Estado,
                       p.PRO_PUBLICADO AS Publicado
                FROM   ALP_PRODUCTO p
                JOIN   ALP_PRODUCTO_CATEGORIA pc ON pc.PRO_PRODUCTO = p.PRO_PRODUCTO
                WHERE  pc.CAT_CATEGORIA = :categoriaId
                ORDER BY p.PRO_NOMBRE";
            return await connection.QueryAsync<ProductoEnCategoria>(sql, new { categoriaId });
        }

        /// <summary>Categorías a las que pertenece un producto.</summary>
        public async Task<IEnumerable<Categoria>> GetCategoriasByProductoAsync(int productoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT c.CAT_CATEGORIA  AS Id,
                       c.CAT_CODIGO     AS Codigo,
                       c.CAT_NOMBRE     AS Nombre,
                       c.CAT_DESCRIPCION AS Descripcion
                FROM   ALP_CATEGORIA c
                JOIN   ALP_PRODUCTO_CATEGORIA pc ON pc.CAT_CATEGORIA = c.CAT_CATEGORIA
                WHERE  pc.PRO_PRODUCTO = :productoId
                ORDER BY c.CAT_NOMBRE";
            return await connection.QueryAsync<Categoria>(sql, new { productoId });
        }

        /// <summary>
        /// Crea una categoría con SQL directo (sin depender del paquete PKG_CATALOGOS).
        /// Verifica unicidad de código, inserta con IDENTITY y obtiene el ID generado.
        /// </summary>
        public async Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string codigo, string nombre, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                // 1. Verificar que el código no exista
                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ALP_CATEGORIA WHERE CAT_CODIGO = UPPER(TRIM(:codigo))",
                    new { codigo });

                if (exists > 0)
                    return (false,
                        $"Ya existe una categoría con el código '{codigo.ToUpper()}'. Usa un nombre diferente.",
                        null);

                // 2. INSERT — CAT_CATEGORIA usa GENERATED ALWAYS AS IDENTITY (PK automático)
                await connection.ExecuteAsync(
                    @"INSERT INTO ALP_CATEGORIA (CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION)
                      VALUES (UPPER(TRIM(:codigo)), TRIM(:nombre), :descripcion)",
                    new { codigo, nombre, descripcion });

                // 3. Recuperar el ID generado por el IDENTITY buscando por el código único
                var insertedId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT CAT_CATEGORIA FROM ALP_CATEGORIA
                      WHERE CAT_CODIGO = UPPER(TRIM(:codigo)) AND ROWNUM = 1",
                    new { codigo });

                return (true, "Categoría creada correctamente.", insertedId > 0 ? insertedId : null);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear la categoría: {ex.Message}", null);
            }
        }

        /// <summary>Actualiza nombre y descripción con SQL directo.</summary>
        public async Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                var rows = await connection.ExecuteAsync(
                    @"UPDATE ALP_CATEGORIA
                      SET CAT_NOMBRE = TRIM(:nombre), CAT_DESCRIPCION = :descripcion
                      WHERE CAT_CATEGORIA = :id",
                    new { nombre, descripcion, id });

                return rows > 0
                    ? (true,  "Categoría actualizada correctamente.")
                    : (false, "Categoría no encontrada.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la categoría si no tiene productos asignados.
        /// Hace DELETE directo en ALP_CATEGORIA (la tabla no tiene columna de estado).
        /// </summary>
        public async Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                // Verificar que no tenga productos
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ALP_PRODUCTO_CATEGORIA WHERE CAT_CATEGORIA = :id AND ROWNUM = 1",
                    new { id });

                if (count > 0)
                    return (false, "No se puede eliminar: la categoría tiene productos asignados.");

                var rows = await connection.ExecuteAsync(
                    "DELETE FROM ALP_CATEGORIA WHERE CAT_CATEGORIA = :id", new { id });

                return rows > 0
                    ? (true, "Categoría eliminada correctamente.")
                    : (false, "Categoría no encontrada.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar: {ex.Message}");
            }
        }
    }
}
