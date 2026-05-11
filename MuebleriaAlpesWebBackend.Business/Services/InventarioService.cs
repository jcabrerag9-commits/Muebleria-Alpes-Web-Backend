using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Connection;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ProductoInventarioService : IProductoInventarioService
    {
        private readonly IProductoInventarioRepository _productoRepository;
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoInventarioService(IProductoInventarioRepository productoRepository, OracleConnectionFactory connectionFactory)
        {
            _productoRepository = productoRepository;
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> CrearProductoAsync(CrearProductoRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _productoRepository.CrearProductoAsync(request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int productoId)
        {
            return await _productoRepository.ObtenerProductoPorIdAsync(productoId);
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosAsync()
        {
            return await _productoRepository.ObtenerTodosAsync();
        }

        public async Task<InventarioResponse<int?>> CrearVarianteAsync(CrearVarianteRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _productoRepository.CrearVarianteAsync(request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly OracleConnectionFactory _connectionFactory;

        public InventarioService(IInventarioRepository inventarioRepository, OracleConnectionFactory connectionFactory)
        {
            _inventarioRepository = inventarioRepository;
            _connectionFactory = connectionFactory;
        }

        public async Task<InventarioResponse<int?>> RegistrarEntradaAsync(MovimientoInventarioRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _inventarioRepository.RegistrarEntradaAsync(request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<InventarioResponse<int?>> RegistrarSalidaAsync(MovimientoInventarioRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _inventarioRepository.RegistrarSalidaAsync(request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<InventarioResponse<int?>> ReservarStockAsync(ReservaStockRequest request)
        {
            // Ejemplo de orquestación transaccional si fuera necesario
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _inventarioRepository.ReservarStockAsync(request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _inventarioRepository.LiberarReservaAsync(reservaId, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<ExistenciaDTO>> ObtenerExistenciasPorProductoAsync(int productoId)
        {
            return await _inventarioRepository.ObtenerExistenciasPorProductoAsync(productoId);
        }
    }
}
