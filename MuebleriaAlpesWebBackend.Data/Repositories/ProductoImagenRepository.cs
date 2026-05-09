using Oracle.ManagedDataAccess.Client;
using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ProductoImagenRepository : IProductoImagenRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoImagenRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden, IDbTransaction? transaction = null)
        {
            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;
            
            if (activeConnection.State != ConnectionState.Open) activeConnection.Open();

            using var command = (OracleCommand)activeConnection.CreateCommand();
            command.CommandText = "PKG_PRODUCTO_IMAGEN.sp_subir_imagen_producto";
            command.CommandType = CommandType.StoredProcedure;
            if (transaction != null) command.Transaction = (OracleTransaction)transaction;

            command.Parameters.Add("p_pro_producto", OracleDbType.Int32).Value = productoId;
            command.Parameters.Add("p_pim_archivo", OracleDbType.Blob).Value = archivo;
            command.Parameters.Add("p_pim_nombre", OracleDbType.Varchar2).Value = nombre;
            command.Parameters.Add("p_pim_content_type", OracleDbType.Varchar2).Value = contentType;
            command.Parameters.Add("p_pim_tamanio", OracleDbType.Int64).Value = tamanio;
            command.Parameters.Add("p_pim_url", OracleDbType.Varchar2).Value = (object?)url ?? DBNull.Value;
            command.Parameters.Add("p_pim_tipo", OracleDbType.Varchar2).Value = (object?)tipo ?? DBNull.Value;
            command.Parameters.Add("p_pim_orden", OracleDbType.Int32).Value = orden;
            
            var pIdGenerado = new OracleParameter("p_id_generado", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add(pIdGenerado);

            command.ExecuteNonQuery();

            return Convert.ToInt32(pIdGenerado.Value.ToString());
        }

        public async Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();

            using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = "PKG_PRODUCTO_IMAGEN.sp_obtener_imagen_producto";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_pim_id", OracleDbType.Int32).Value = imagenId;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            if (reader.Read())
            {
                var dto = new ProductoImagenDTO
                {
                    ImagenId = imagenId,
                    ContentType = reader.GetString(reader.GetOrdinal("PIM_CONTENT_TYPE")),
                    NombreArchivo = reader.GetString(reader.GetOrdinal("PIM_NOMBRE_ARCHIVO")),
                    Tamanio = Convert.ToInt64(reader.GetValue(reader.GetOrdinal("PIM_TAMANIO"))),
                    ProductoId = reader.GetInt32(reader.GetOrdinal("PRO_PRODUCTO")),
                    // PIM_TIPO no viene en sp_obtener_imagen_producto para optimizar streaming
                };

                int blobOrdinal = reader.GetOrdinal("PIM_ARCHIVO");
                if (!reader.IsDBNull(blobOrdinal))
                {
                    long length = reader.GetBytes(blobOrdinal, 0, null, 0, 0);
                    byte[] buffer = new byte[length];
                    reader.GetBytes(blobOrdinal, 0, buffer, 0, (int)length);
                    dto.Archivo = buffer;
                }

                return dto;
            }

            return null;
        }

        public async Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId)
        {
            var imagenes = new List<ProductoImagenListadoDTO>();

            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();

            using var command = (OracleCommand)connection.CreateCommand();
            command.CommandText = "PKG_PRODUCTO_IMAGEN.sp_listar_imagenes_producto";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_pro_producto", OracleDbType.Int32).Value = productoId;
            command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using var reader = command.ExecuteReader();
            
            // Precalcular ordinales para performance y seguridad de nulos
            int ordImagenId = reader.GetOrdinal("ImagenId");
            int ordProductoId = reader.GetOrdinal("ProductoId");
            int ordNombreArchivo = reader.GetOrdinal("NombreArchivo");
            int ordContentType = reader.GetOrdinal("ContentType");
            int ordTamanio = reader.GetOrdinal("Tamanio");
            int ordUrl = reader.GetOrdinal("Url");
            int ordTipo = reader.GetOrdinal("Tipo");
            int ordOrden = reader.GetOrdinal("Orden");
            int ordFechaCarga = reader.GetOrdinal("FechaCarga");

            while (reader.Read())
            {
                imagenes.Add(new ProductoImagenListadoDTO
                {
                    ImagenId = reader.GetInt32(ordImagenId),
                    ProductoId = reader.GetInt32(ordProductoId),
                    NombreArchivo = reader.IsDBNull(ordNombreArchivo) ? string.Empty : reader.GetString(ordNombreArchivo),
                    ContentType = reader.IsDBNull(ordContentType) ? string.Empty : reader.GetString(ordContentType),
                    Tamanio = reader.IsDBNull(ordTamanio) ? 0 : Convert.ToInt64(reader.GetValue(ordTamanio)),
                    Url = reader.IsDBNull(ordUrl) ? null : reader.GetString(ordUrl),
                    Tipo = reader.IsDBNull(ordTipo) ? null : reader.GetString(ordTipo),
                    Orden = reader.IsDBNull(ordOrden) ? 0 : reader.GetInt32(ordOrden),
                    FechaCarga = reader.IsDBNull(ordFechaCarga) ? DateTime.MinValue : reader.GetDateTime(ordFechaCarga)
                });
            }

            return imagenes;
        }

        public async Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId)
        {
            // Consulta optimizada para traer solo la primera activa de tipo PRINCIPAL o la primera en orden
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT PIM_PRODUCTO_IMAGEN 
                          FROM ALP_PRODUCTO_IMAGEN 
                          WHERE PRO_PRODUCTO = :productoId 
                            AND PIM_ESTADO = 'ACTIVO' 
                          ORDER BY CASE WHEN PIM_TIPO = 'PRINCIPAL' THEN 1 ELSE 2 END, PIM_ORDEN ASC 
                          FETCH FIRST 1 ROW ONLY";
            
            int? imagenId = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { productoId });
            
            if (imagenId.HasValue)
            {
                return await ObtenerImagenAsync(imagenId.Value);
            }
            
            return null;
        }

        public async Task<bool> EliminarImagenAsync(int imagenId, IDbTransaction? transaction = null)
        {
            using var connection = transaction?.Connection == null ? _connectionFactory.CreateConnection() : null;
            var activeConnection = transaction?.Connection ?? connection!;

            if (activeConnection.State != ConnectionState.Open) activeConnection.Open();

            using var command = (OracleCommand)activeConnection.CreateCommand();
            command.CommandText = "PKG_PRODUCTO_IMAGEN.sp_eliminar_imagen_producto";
            command.CommandType = CommandType.StoredProcedure;
            if (transaction != null) command.Transaction = (OracleTransaction)transaction;

            command.Parameters.Add("p_pim_id", OracleDbType.Int32).Value = imagenId;
            var pFilas = new OracleParameter("p_filas_afect", OracleDbType.Int32, ParameterDirection.Output);
            command.Parameters.Add(pFilas);

            command.ExecuteNonQuery();

            return Convert.ToInt32(pFilas.Value.ToString()) > 0;
        }
    }
}
