using System.Collections.Generic;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Connection;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly OracleConnectionFactory _connectionFactory;

        public ProductoService(IProductoRepository productoRepository, OracleConnectionFactory connectionFactory)
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

        public async Task<InventarioResponse<bool>> ActualizarProductoAsync(int productoId, ActualizarProductoRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _productoRepository.ActualizarProductoAsync(productoId, request, transaction);
                transaction.Commit();
                return response;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
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
        private readonly IBodegaService _bodegaService;
        private readonly OracleConnectionFactory _connectionFactory;

        public InventarioService(IInventarioRepository inventarioRepository, IBodegaService bodegaService, OracleConnectionFactory connectionFactory)
        {
            _inventarioRepository = inventarioRepository;
            _bodegaService = bodegaService;
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

        public async Task<InventarioResponse<bool>> LiberarReservaAsync(int reservaId, int? usuarioId = null, string? observacion = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var response = await _inventarioRepository.LiberarReservaAsync(reservaId, usuarioId, observacion, transaction);
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

        /// <summary>
        /// Libera stock respetando las reglas de negocio ERP por Motivo.
        ///
        /// MOTIVOS LIBERABLES MANUALMENTE:
        ///   APARTADO_MANUAL   → bloqueado por vendedor, puede cancelarse.
        ///   RESERVA_TEMPORAL  → bloqueo sin origen comercial, puede cancelarse.
        ///
        /// MOTIVOS BLOQUEADOS:
        ///   CARRITO           → administrado por el sistema/job de expiración.
        ///   ORDEN_CONFIRMADA  → stock comprometido comercialmente. Solo se libera al despachar.
        /// </summary>
        public async Task<InventarioResponse<bool>> ValidarYLiberarReservaAsync(int reservaId)
        {
            // FASE B ERP: Ahora delegamos la validación a Oracle SP_LIBERAR_STOCK_RESERVADO
            // El SP lanzará ORA-20501 si el motivo es Carrito/Orden y no se envía el flag de sistema.
            // Aquí enviamos UsuarioId = 1 (Sistema/Admin por defecto) y observación.
            
            System.Console.WriteLine($"[SERVICE RESERVAS] Delegando liberación de reserva ID: {reservaId} a Oracle ERP.");
            
            return await LiberarReservaAsync(reservaId, usuarioId: 1, observacion: "Liberación manual de reserva de stock.");
        }

        public async Task<IEnumerable<ReservaDTO>> ObtenerReservasPorProductoAsync(int productoId)
        {
            // Lectura pura — sin transacción, es solo SELECT
            return await _inventarioRepository.ObtenerReservasPorProductoAsync(productoId);
        }

        public async Task<IEnumerable<BodegaDTO>> ObtenerBodegasAsync()
        {
            return await _bodegaService.ListarBodegasAsync(soloActivas: true);
        }

        public async Task<IEnumerable<KardexDTO>> ObtenerKardexPorProductoAsync(int productoId)
        {
            return await _inventarioRepository.ObtenerKardexPorProductoAsync(productoId);
        }

        public async Task<IEnumerable<KardexDTO>> ObtenerMovimientosGlobalesAsync(MovimientoFiltroRequest filtro)
        {
            return await _inventarioRepository.ObtenerMovimientosGlobalesAsync(filtro);
        }
    }
}
