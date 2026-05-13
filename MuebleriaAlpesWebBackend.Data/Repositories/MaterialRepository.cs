using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Text.RegularExpressions;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public MaterialRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Material>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT MAT_MATERIAL   AS Id,
                       MAT_CODIGO     AS Codigo,
                       MAT_NOMBRE     AS Nombre,
                       MAT_DESCRIPCION AS Descripcion,
                       MAT_ESTADO     AS Estado
                FROM   ALP_MATERIAL
                ORDER  BY MAT_NOMBRE";
            return await connection.QueryAsync<Material>(sql);
        }

        public async Task<Material?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT MAT_MATERIAL AS Id, MAT_CODIGO AS Codigo, MAT_NOMBRE AS Nombre,
                       MAT_DESCRIPCION AS Descripcion, MAT_ESTADO AS Estado
                FROM   ALP_MATERIAL WHERE MAT_MATERIAL = :id";
            return await connection.QueryFirstOrDefaultAsync<Material>(sql, new { id });
        }

        public async Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(string nombre, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                var codigo = GenerarCodigo(nombre);

                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ALP_MATERIAL WHERE MAT_CODIGO = UPPER(TRIM(:codigo))",
                    new { codigo });

                if (exists > 0)
                    return (false,
                        $"Ya existe un material con el código '{codigo.ToUpper()}'. Usa un nombre diferente.",
                        null);

                await connection.ExecuteAsync(
                    @"INSERT INTO ALP_MATERIAL (MAT_CODIGO, MAT_NOMBRE, MAT_DESCRIPCION, MAT_ESTADO)
                      VALUES (UPPER(TRIM(:codigo)), TRIM(:nombre), :descripcion, 'ACTIVO')",
                    new { codigo, nombre, descripcion });

                var insertedId = await connection.ExecuteScalarAsync<int>(
                    "SELECT MAT_MATERIAL FROM ALP_MATERIAL WHERE MAT_CODIGO = UPPER(TRIM(:codigo)) AND ROWNUM = 1",
                    new { codigo });

                return (true, "Material creado correctamente.", insertedId > 0 ? insertedId : null);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear el material: {ex.Message}", null);
            }
        }

        public async Task<(bool Ok, string Mensaje)> UpdateAsync(int id, string nombre, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                var rows = await connection.ExecuteAsync(
                    @"UPDATE ALP_MATERIAL
                      SET MAT_NOMBRE = TRIM(:nombre), MAT_DESCRIPCION = :descripcion
                      WHERE MAT_MATERIAL = :id",
                    new { nombre, descripcion, id });

                return rows > 0
                    ? (true,  "Material actualizado correctamente.")
                    : (false, "Material no encontrado.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar el material: {ex.Message}");
            }
        }

        public async Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                // Verificar si tiene productos asignados
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ALP_PRODUCTO_MATERIAL WHERE MAT_MATERIAL = :id AND ROWNUM = 1",
                    new { id });

                if (count > 0)
                    return (false, "No se puede eliminar: el material tiene productos asignados.");

                // Soft-delete
                var rows = await connection.ExecuteAsync(
                    "UPDATE ALP_MATERIAL SET MAT_ESTADO = 'INACTIVO' WHERE MAT_MATERIAL = :id",
                    new { id });

                return rows > 0
                    ? (true,  "Material eliminado correctamente.")
                    : (false, "Material no encontrado.");
            }
            catch
            {
                // Si la tabla de relación no existe, hacer soft-delete directo
                try
                {
                    var rows = await connection.ExecuteAsync(
                        "UPDATE ALP_MATERIAL SET MAT_ESTADO = 'INACTIVO' WHERE MAT_MATERIAL = :id",
                        new { id });
                    return rows > 0 ? (true, "Material eliminado correctamente.") : (false, "Material no encontrado.");
                }
                catch (Exception ex2)
                {
                    return (false, $"Error al eliminar el material: {ex2.Message}");
                }
            }
        }

        private static string GenerarCodigo(string nombre)
        {
            var limpio = Regex.Replace(nombre.Trim().ToUpper(), @"[^A-Z0-9]", "_");
            limpio = Regex.Replace(limpio, @"_+", "_").Trim('_');
            return limpio.Length > 20 ? limpio[..20] : limpio;
        }
    }
}
