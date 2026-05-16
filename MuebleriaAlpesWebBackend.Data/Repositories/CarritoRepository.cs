using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Carrito;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class CarritoRepository : ICarritoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public CarritoRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BaseResponse<AgregarProductoCarritoDataDto>> AgregarProductoAsync(AgregarProductoCarritoRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_AGREGAR_PRODUCTO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_cli_cliente", OracleDbType.Int32, request.ClienteId, ParameterDirection.Input);
            cmd.Parameters.Add("p_pro_producto", OracleDbType.Int32, request.ProductoId, ParameterDirection.Input);
            cmd.Parameters.Add("p_cantidad", OracleDbType.Int32, request.Cantidad, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            var pCarritoId = new OracleParameter("p_carrito_id", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pCarritoId);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse<AgregarProductoCarritoDataDto>
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty,
                Data = new AgregarProductoCarritoDataDto
                {
                    CarritoId = pCarritoId.Value is OracleDecimal dec && !dec.IsNull ? dec.ToInt32() : 0
                }
            };
        }

        public async Task<BaseResponse> ActualizarCantidadAsync(ActualizarCantidadCarritoRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_ACTUALIZAR_CANTIDAD";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_detalle_id", OracleDbType.Int32, request.DetalleId, ParameterDirection.Input);
            cmd.Parameters.Add("p_nueva_cantidad", OracleDbType.Int32, request.NuevaCantidad, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty
            };
        }

        public async Task<BaseResponse> EliminarProductoAsync(int detalleId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_ELIMINAR_PRODUCTO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_detalle_id", OracleDbType.Int32, detalleId, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty
            };
        }

        public async Task<BaseResponse> VaciarCarritoAsync(int carritoId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_VACIAR";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_carrito_id", OracleDbType.Int32, carritoId, ParameterDirection.Input);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty
            };
        }

        public async Task<BaseResponse<CalcularTotalCarritoDataDto>> CalcularTotalAsync(int carritoId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_CALCULAR_TOTAL";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_carrito_id", OracleDbType.Int32, carritoId, ParameterDirection.Input);

            var pSubtotal = new OracleParameter("p_subtotal", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pSubtotal);

            var pImpuestos = new OracleParameter("p_impuestos", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pImpuestos);

            var pTotal = new OracleParameter("p_total", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pTotal);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse<CalcularTotalCarritoDataDto>
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty,
                Data = new CalcularTotalCarritoDataDto
                {
                    Subtotal = pSubtotal.Value is OracleDecimal subDec && !subDec.IsNull ? subDec.Value : 0,
                    Impuestos = pImpuestos.Value is OracleDecimal impDec && !impDec.IsNull ? impDec.Value : 0,
                    Total = pTotal.Value is OracleDecimal totDec && !totDec.IsNull ? totDec.Value : 0
                }
            };
        }

        public async Task<BaseResponse<ConvertirOrdenCarritoDataDto>> ConvertirOrdenAsync(ConvertirOrdenCarritoRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_CONVERTIR_ORDEN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName  = true;

            // --- Parámetros de Entrada ---
            cmd.Parameters.Add("p_carrito_id",  OracleDbType.Int32, request.CarritoId, ParameterDirection.Input);
            cmd.Parameters.Add("p_canal_venta", OracleDbType.Int32, request.CanalVenta, ParameterDirection.Input);

            // --- Parámetros de Salida ---
            var pOrdenId   = new OracleParameter("p_orden_id",  OracleDbType.Decimal,              ParameterDirection.Output);
            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100,  null, ParameterDirection.Output);
            var pMensaje   = new OracleParameter("p_mensaje",   OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            
            cmd.Parameters.Add(pOrdenId);
            cmd.Parameters.Add(pResultado);
            cmd.Parameters.Add(pMensaje);

            try
            {
                await cmd.ExecuteNonQueryAsync();

                var resultado = pResultado.Value?.ToString() ?? "ERROR";
                var ordenId   = pOrdenId.Value is OracleDecimal dec && !dec.IsNull ? (int?)dec.ToInt32() : null;

                return new BaseResponse<ConvertirOrdenCarritoDataDto>
                {
                    Resultado = resultado,
                    Mensaje   = pMensaje.Value?.ToString() ?? string.Empty,
                    Data      = new ConvertirOrdenCarritoDataDto { OrdenId = ordenId }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ConvertirOrdenCarritoDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje   = $"Error fatal en la transacción de venta: {ex.Message}",
                    Data      = new ConvertirOrdenCarritoDataDto { OrdenId = null }
                };
            }
        }

        public async Task<BaseResponse<ObtenerCarritoClienteDataDto>> ObtenerCarritoClienteAsync(int clienteId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_OBTENER_CARRITO_CLIENTE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            // --- Parámetro IN ---
            cmd.Parameters.Add("p_cliente_id", OracleDbType.Int32, clienteId, ParameterDirection.Input);

            // --- Cursores OUT (nombres exactos del SP) ---
            var pCurCabecera = new OracleParameter("p_cur_cabecera", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add(pCurCabecera);

            var pCurDetalle = new OracleParameter("p_cur_detalle", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add(pCurDetalle);

            await cmd.ExecuteNonQueryAsync();

            var data = new ObtenerCarritoClienteDataDto();

            // ── Leer cursor cabecera ──
            // Columnas: CAR_CARRITO, CAR_SUBTOTAL, CAR_FECHA_CREACION
            if (pCurCabecera.Value is OracleRefCursor refCursorCabecera)
            {
                using var readerCabecera = refCursorCabecera.GetDataReader();
                if (await readerCabecera.ReadAsync())
                {
                    data.Cabecera = new CarritoCabeceraDto
                    {
                        CarritoId     = readerCabecera.GetInt32(readerCabecera.GetOrdinal("CAR_CARRITO")),
                        Subtotal      = readerCabecera.IsDBNull(readerCabecera.GetOrdinal("CAR_SUBTOTAL"))
                                            ? 0
                                            : readerCabecera.GetDecimal(readerCabecera.GetOrdinal("CAR_SUBTOTAL")),
                        FechaCreacion = readerCabecera.GetDateTime(readerCabecera.GetOrdinal("CAR_FECHA_CREACION"))
                    };
                }
            }

            // ── Leer cursor detalle ──
            // Columnas: CAD_CARRITO_DETALLE, PRO_PRODUCTO, PRO_NOMBRE, CAD_CANTIDAD, CAD_PRECIO_UNITARIO, CAD_SUBTOTAL
            if (pCurDetalle.Value is OracleRefCursor refCursorDetalle)
            {
                using var readerDetalle = refCursorDetalle.GetDataReader();
                while (await readerDetalle.ReadAsync())
                {
                    data.Detalles.Add(new CarritoDetalleDto
                    {
                        DetalleId      = readerDetalle.GetInt32(readerDetalle.GetOrdinal("CAD_CARRITO_DETALLE")),
                        ProductoId     = readerDetalle.GetInt32(readerDetalle.GetOrdinal("PRO_PRODUCTO")),
                        NombreProducto = readerDetalle.IsDBNull(readerDetalle.GetOrdinal("PRO_NOMBRE"))
                                            ? string.Empty
                                            : readerDetalle.GetString(readerDetalle.GetOrdinal("PRO_NOMBRE")),
                        Cantidad       = readerDetalle.GetInt32(readerDetalle.GetOrdinal("CAD_CANTIDAD")),
                        PrecioUnitario = readerDetalle.GetDecimal(readerDetalle.GetOrdinal("CAD_PRECIO_UNITARIO")),
                        Subtotal       = readerDetalle.GetDecimal(readerDetalle.GetOrdinal("CAD_SUBTOTAL"))
                    });
                }
            }

            // El SP no devuelve p_resultado/p_mensaje; lo construimos según si encontró datos
            bool tieneCarrito = data.Cabecera != null;

            return new BaseResponse<ObtenerCarritoClienteDataDto>
            {
                Resultado = tieneCarrito ? "EXITO" : "ERROR",
                Mensaje = tieneCarrito ? "Carrito obtenido correctamente" : "No se encontró carrito para el cliente",
                Data = data
            };
        }
    }
}
