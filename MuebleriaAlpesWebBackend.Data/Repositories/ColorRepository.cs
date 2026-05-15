using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Text.RegularExpressions;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ColorRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>ALP_COLOR: COL_COLOR, COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_DESCRIPCION, COL_ESTADO</summary>
        public async Task<IEnumerable<Color>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT COL_COLOR        AS Id,
                       COL_CODIGO       AS Codigo,
                       COL_NOMBRE       AS Nombre,
                       COL_CODIGO_HEX   AS HexColor,
                       COL_DESCRIPCION  AS Descripcion,
                       COL_ESTADO       AS Estado
                FROM   ALP_COLOR
                ORDER  BY COL_NOMBRE";
            return await connection.QueryAsync<Color>(sql);
        }

        public async Task<Color?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT COL_COLOR AS Id, COL_CODIGO AS Codigo, COL_NOMBRE AS Nombre,
                       COL_CODIGO_HEX AS HexColor, COL_DESCRIPCION AS Descripcion, COL_ESTADO AS Estado
                FROM   ALP_COLOR WHERE COL_COLOR = :id";
            return await connection.QueryFirstOrDefaultAsync<Color>(sql, new { id });
        }

        /// <summary>
        /// Crea un color con SQL directo.
        /// El código se genera desde el nombre (CAOBA, AZUL_MARINO…).
        /// El hex debe iniciar con '#' — el input type="color" del browser lo garantiza.
        /// </summary>
        public async Task<(bool Ok, string Mensaje, int? Id)> CreateAsync(
            string nombre, string hexColor, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                // Asegurar que el hex inicia con #
                if (!hexColor.StartsWith("#"))
                    hexColor = "#" + hexColor;

                // Generar código único a partir del nombre
                var codigo = GenerarCodigo(nombre);

                // Verificar unicidad del código
                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ALP_COLOR WHERE COL_CODIGO = UPPER(TRIM(:codigo))",
                    new { codigo });

                if (exists > 0)
                    return (false,
                        $"Ya existe un color con el código '{codigo.ToUpper()}'. Usa un nombre diferente.",
                        null);

                // INSERT
                await connection.ExecuteAsync(
                    @"INSERT INTO ALP_COLOR (COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_DESCRIPCION, COL_ESTADO)
                      VALUES (UPPER(TRIM(:codigo)), TRIM(:nombre), :hexColor, :descripcion, 'ACTIVO')",
                    new { codigo, nombre, hexColor, descripcion });

                // Recuperar el ID generado
                var insertedId = await connection.ExecuteScalarAsync<int>(
                    "SELECT COL_COLOR FROM ALP_COLOR WHERE COL_CODIGO = UPPER(TRIM(:codigo)) AND ROWNUM = 1",
                    new { codigo });

                return (true, "Color creado correctamente.", insertedId > 0 ? insertedId : null);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear el color: {ex.Message}", null);
            }
        }

        /// <summary>Actualiza nombre, hex y descripción con SQL directo.</summary>
        public async Task<(bool Ok, string Mensaje)> UpdateAsync(
            int id, string nombre, string hexColor, string? descripcion)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                if (!hexColor.StartsWith("#"))
                    hexColor = "#" + hexColor;

                var rows = await connection.ExecuteAsync(
                    @"UPDATE ALP_COLOR
                      SET COL_NOMBRE       = TRIM(:nombre),
                          COL_CODIGO_HEX   = :hexColor,
                          COL_DESCRIPCION  = :descripcion
                      WHERE COL_COLOR = :id",
                    new { nombre, hexColor, descripcion, id });

                return rows > 0
                    ? (true,  "Color actualizado correctamente.")
                    : (false, "Color no encontrado.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar el color: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina (soft-delete) marcando el color como INACTIVO.
        /// Primero verifica que no tenga productos asignados.
        /// </summary>
        public async Task<(bool Ok, string Mensaje)> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            try
            {
                // Verificar productos asignados (tabla ALP_PRODUCTO_COLOR)
                var count = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM ALP_PRODUCTO_COLOR WHERE COL_COLOR = :id AND ROWNUM = 1",
                    new { id });

                if (count > 0)
                    return (false, "No se puede eliminar: el color tiene productos asignados.");

                // Soft-delete: marcar INACTIVO
                var rows = await connection.ExecuteAsync(
                    "UPDATE ALP_COLOR SET COL_ESTADO = 'INACTIVO' WHERE COL_COLOR = :id",
                    new { id });

                return rows > 0
                    ? (true,  "Color eliminado correctamente.")
                    : (false, "Color no encontrado.");
            }
            catch (Exception ex)
            {
                // Si ALP_PRODUCTO_COLOR no existe, intentar eliminar directamente
                try
                {
                    var rows = await connection.ExecuteAsync(
                        "UPDATE ALP_COLOR SET COL_ESTADO = 'INACTIVO' WHERE COL_COLOR = :id",
                        new { id });
                    return rows > 0 ? (true, "Color eliminado correctamente.") : (false, "Color no encontrado.");
                }
                catch
                {
                    return (false, $"Error al eliminar el color: {ex.Message}");
                }
            }
        }

        /// <summary>Genera código a partir del nombre: AZUL_MARINO, CAOBA, etc.</summary>
        private static string GenerarCodigo(string nombre)
        {
            var limpio = Regex.Replace(nombre.Trim().ToUpper(), @"[^A-Z0-9]", "_");
            limpio = Regex.Replace(limpio, @"_+", "_").Trim('_');
            return limpio.Length > 20 ? limpio[..20] : limpio;
        }
    }
}
