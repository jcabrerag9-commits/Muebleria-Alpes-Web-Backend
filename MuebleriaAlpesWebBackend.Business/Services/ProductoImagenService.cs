using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ProductoImagenService : IProductoImagenService
    {
        private readonly IProductoImagenRepository _repository;
        private readonly OracleConnectionFactory _connectionFactory;

        // Hardening: Límites y validaciones enterprise
        private const long MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
        private static readonly HashSet<string> TIPOS_VALIDOS = new(StringComparer.OrdinalIgnoreCase)
        {
            "PRINCIPAL", "SECUNDARIA", "DETALLE", "360", "VIDEO"
        };

        public ProductoImagenService(IProductoImagenRepository repository, OracleConnectionFactory connectionFactory)
        {
            _repository = repository;
            _connectionFactory = connectionFactory;
        }

        public async Task<ApiResponse<int>> SubirImagenAsync(int productoId, byte[] archivo, string nombre, string contentType, long tamanio, string? url, string? tipo, int orden)
        {
            // Validaciones iniciales (Hardening)
            if (archivo == null || archivo.Length == 0)
                return Error<int>("El archivo no puede estar vacío.");

            if (archivo.Length > MAX_FILE_SIZE)
                return Error<int>($"El archivo excede el límite permitido de {MAX_FILE_SIZE / 1024 / 1024}MB.");

            // Validación de Tipo (Evitar ORA-02290)
            if (string.IsNullOrWhiteSpace(tipo) || !TIPOS_VALIDOS.Contains(tipo))
            {
                return Error<int>($"Tipo de imagen '{tipo}' no válido. Permitidos: {string.Join(", ", TIPOS_VALIDOS)}");
            }

            tipo = tipo.ToUpper();

            System.Console.WriteLine($"[Service Log] SubirImagenAsync llamado para Producto: {productoId}");
            System.Console.WriteLine($"[Service Log] UrlOpcional recibida: {url ?? "null"}");

            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                int idGenerado = await _repository.SubirImagenAsync(
                    productoId,
                    archivo,
                    nombre,
                    contentType,
                    tamanio,
                    url,
                    tipo,
                    orden,
                    transaction
                );

                transaction.Commit();

                return new ApiResponse<int>
                {
                    Success = true,
                    Message = "Imagen cargada correctamente.",
                    Data = idGenerado
                };
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                transaction.Rollback();
                return Error<int>(ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Error<int>($"Error interno en carga: {ex.Message}");
            }
        }

        public async Task<ProductoImagenDTO?> ObtenerImagenAsync(int imagenId)
        {
            return await _repository.ObtenerImagenAsync(imagenId);
        }

        public async Task<IEnumerable<ProductoImagenListadoDTO>> ListarPorProductoAsync(int productoId)
        {
            return await _repository.ListarPorProductoAsync(productoId);
        }

        public async Task<ProductoImagenDTO?> ObtenerPrincipalPorProductoAsync(int productoId)
        {
            return await _repository.ObtenerPrincipalPorProductoAsync(productoId);
        }

        public async Task<ApiResponse<bool>> EliminarImagenAsync(int imagenId)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool eliminado = await _repository.EliminarImagenAsync(imagenId, transaction);
                transaction.Commit();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Imagen desactivada correctamente.",
                    Data = eliminado
                };
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                transaction.Rollback();
                return Error<bool>(ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Error<bool>($"Error interno en eliminación: {ex.Message}");
            }
        }

        private ApiResponse<T> Error<T>(string mensaje)
        {
            return new ApiResponse<T> { Success = false, Message = mensaje };
        }
    }
}
