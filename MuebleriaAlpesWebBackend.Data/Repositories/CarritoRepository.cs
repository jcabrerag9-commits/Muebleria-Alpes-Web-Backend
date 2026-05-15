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

            // ── 0. Normalizar CanalVenta: si el ID no existe en la tabla, usar el primero disponible ──
            int canalVentaId = await ResolverCanalVentaAsync(connection, request.CanalVenta);
            if (canalVentaId == 0)
                return new BaseResponse<ConvertirOrdenCarritoDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje   = "No hay canales de venta configurados en el sistema. Contacta al administrador.",
                    Data      = new ConvertirOrdenCarritoDataDto { OrdenId = null }
                };

            // ── 1. Capturar items del carrito ANTES de que el SP los elimine ──
            var itemsCarrito = await ObtenerItemsCarritoDirectoAsync(connection, request.CarritoId);

            // ── 2. Garantizar que cada producto tenga un precio ACTIVO en ALP_PRODUCTO_PRECIO ──
            //      (el SP hace SELECT INTO sobre esa tabla — si no hay precio activo, lanza ORA-01403)
            await EnsurePreciosActivosAsync(connection, itemsCarrito);

            // ── 2b. Garantizar que cada producto tenga fila en ALP_EXISTENCIA ──
            //       (el SP verifica stock via SELECT INTO — si no existe fila, lanza ORA-01403)
            await EnsureExistenciasAsync(connection, itemsCarrito);

            // ── 3. Ejecutar SP de conversión ──
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CARRITO_CONVERTIR_ORDEN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName  = true;

            cmd.Parameters.Add("p_carrito_id", OracleDbType.Int32, request.CarritoId, ParameterDirection.Input);
            cmd.Parameters.Add("p_canal_venta", OracleDbType.Int32, canalVentaId,     ParameterDirection.Input);

            var pOrdenId   = new OracleParameter("p_orden_id",  OracleDbType.Decimal,              ParameterDirection.Output);
            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100,  null, ParameterDirection.Output);
            var pMensaje   = new OracleParameter("p_mensaje",   OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pOrdenId);
            cmd.Parameters.Add(pResultado);
            cmd.Parameters.Add(pMensaje);

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (OracleException oex) when (oex.Number == 1403)
            {
                // ORA-01403: el SP hizo SELECT INTO y no encontró datos (precio, canal, estado de orden, etc.)
                return new BaseResponse<ConvertirOrdenCarritoDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje   = "No se pudo procesar la orden: uno o más productos no tienen datos de precio o configuración completos. Contacta al administrador.",
                    Data      = new ConvertirOrdenCarritoDataDto { OrdenId = null }
                };
            }

            var resultado = pResultado.Value?.ToString() ?? string.Empty;
            var ordenId   = pOrdenId.Value is OracleDecimal dec && !dec.IsNull ? (int?)dec.ToInt32() : null;

            // ── 4. Si la orden se creó, descontar inventario (best-effort) ──
            if (resultado == "EXITO" && itemsCarrito.Count > 0)
                await DescontarInventarioAsync(connection, itemsCarrito);

            return new BaseResponse<ConvertirOrdenCarritoDataDto>
            {
                Resultado = resultado,
                Mensaje   = pMensaje.Value?.ToString() ?? string.Empty,
                Data      = new ConvertirOrdenCarritoDataDto { OrdenId = ordenId }
            };
        }

        /// <summary>
        /// Devuelve el CVE_CANAL_VENTA real a usar. Si el ID solicitado no existe en ALP_CANAL_VENTA,
        /// devuelve el primero disponible. Retorna 0 si la tabla está vacía.
        /// </summary>
        private async Task<int> ResolverCanalVentaAsync(OracleConnection conn, int canalVentaId)
        {
            try
            {
                // ¿Existe el ID solicitado?
                using var cmdCheck = conn.CreateCommand();
                cmdCheck.CommandText = "SELECT CVE_CANAL_VENTA FROM ALP_CANAL_VENTA WHERE CVE_CANAL_VENTA = :id AND ROWNUM = 1";
                cmdCheck.Parameters.Add("id", OracleDbType.Int32, canalVentaId, ParameterDirection.Input);
                var found = await cmdCheck.ExecuteScalarAsync();
                if (found != null && found != DBNull.Value)
                    return canalVentaId;

                // No existe → usar el primero disponible
                using var cmdFirst = conn.CreateCommand();
                cmdFirst.CommandText = "SELECT CVE_CANAL_VENTA FROM ALP_CANAL_VENTA WHERE ROWNUM = 1";
                var first = await cmdFirst.ExecuteScalarAsync();
                return (first != null && first != DBNull.Value) ? Convert.ToInt32(first) : 0;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Para cada producto del carrito, garantiza que exista al menos un registro con PPR_ESTADO = 'ACTIVO'
        /// en ALP_PRODUCTO_PRECIO.  Si existe un precio inactivo, lo reactiva; si no hay ningún precio,
        /// crea uno via PKG_PRECIOS.sp_registrar_precio_producto (best-effort, no lanza excepción).
        /// </summary>
        private async Task EnsurePreciosActivosAsync(OracleConnection conn, List<(int ProductoId, int Cantidad)> items)
        {
            if (items.Count == 0) return;

            // Obtener primer MON_MONEDA disponible (necesario para insertar precios nuevos)
            int monedaId = 1;
            try
            {
                using var cmdMon = conn.CreateCommand();
                cmdMon.CommandText = "SELECT MON_MONEDA FROM ALP_MONEDA WHERE ROWNUM = 1";
                var m = await cmdMon.ExecuteScalarAsync();
                if (m != null && m != DBNull.Value) monedaId = Convert.ToInt32(m);
            }
            catch { }

            foreach (var (productoId, _) in items)
            {
                try
                {
                    // ¿Ya tiene precio ACTIVO?
                    using var cmdActive = conn.CreateCommand();
                    cmdActive.CommandText = @"SELECT COUNT(1) FROM ALP_PRODUCTO_PRECIO
                                              WHERE PRO_PRODUCTO = :pId AND PPR_ESTADO = 'ACTIVO' AND ROWNUM = 1";
                    cmdActive.Parameters.Add("pId", OracleDbType.Int32, productoId, ParameterDirection.Input);
                    var cnt = Convert.ToInt32(await cmdActive.ExecuteScalarAsync() ?? 0);
                    if (cnt > 0) continue;  // precio activo existe → OK

                    // ¿Existe algún precio (inactivo)?
                    using var cmdAny = conn.CreateCommand();
                    cmdAny.CommandText = @"SELECT PPR_PRECIO, PPR_TIPO
                                           FROM ALP_PRODUCTO_PRECIO
                                           WHERE PRO_PRODUCTO = :pId AND ROWNUM = 1";
                    cmdAny.Parameters.Add("pId", OracleDbType.Int32, productoId, ParameterDirection.Input);

                    decimal precio = 1.00m;
                    string  tipo   = "REGULAR";
                    bool    hayPrecio = false;

                    using (var r = await cmdAny.ExecuteReaderAsync())
                    {
                        if (await r.ReadAsync())
                        {
                            hayPrecio = true;
                            if (!r.IsDBNull(0)) precio = r.GetDecimal(0);
                            if (!r.IsDBNull(1)) tipo   = r.GetString(1);
                        }
                    }

                    if (hayPrecio)
                    {
                        // Reactivar el precio existente
                        using var cmdUpd = conn.CreateCommand();
                        cmdUpd.CommandText = @"UPDATE ALP_PRODUCTO_PRECIO
                                               SET    PPR_ESTADO = 'ACTIVO', PPR_FECHA_INICIO = CURRENT_DATE
                                               WHERE  PRO_PRODUCTO = :pId AND ROWNUM = 1";
                        cmdUpd.Parameters.Add("pId", OracleDbType.Int32, productoId, ParameterDirection.Input);
                        await cmdUpd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // Sin ningún precio → crear uno via SP de precios
                        using var cmdIns = conn.CreateCommand();
                        cmdIns.CommandText = "PKG_PRECIOS.sp_registrar_precio_producto";
                        cmdIns.CommandType = CommandType.StoredProcedure;
                        cmdIns.BindByName  = true;
                        cmdIns.Parameters.Add("p_producto",      OracleDbType.Int32,    productoId,   ParameterDirection.Input);
                        var pVar = new OracleParameter("p_variante", OracleDbType.Int32) { Value = DBNull.Value, Direction = ParameterDirection.Input };
                        cmdIns.Parameters.Add(pVar);
                        cmdIns.Parameters.Add("p_moneda",        OracleDbType.Int32,    monedaId,     ParameterDirection.Input);
                        cmdIns.Parameters.Add("p_tipo",          OracleDbType.Varchar2,  tipo,         ParameterDirection.Input);
                        cmdIns.Parameters.Add("p_precio",        OracleDbType.Decimal,  precio,       ParameterDirection.Input);
                        var pOfe = new OracleParameter("p_precio_oferta", OracleDbType.Decimal) { Value = DBNull.Value, Direction = ParameterDirection.Input };
                        cmdIns.Parameters.Add(pOfe);
                        cmdIns.Parameters.Add("p_fecha_inicio",  OracleDbType.Date,     DateTime.Today, ParameterDirection.Input);
                        var pFin = new OracleParameter("p_fecha_fin", OracleDbType.Date) { Value = DBNull.Value, Direction = ParameterDirection.Input };
                        cmdIns.Parameters.Add(pFin);
                        cmdIns.Parameters.Add("p_id_nuevo", OracleDbType.Int32, ParameterDirection.Output);
                        await cmdIns.ExecuteNonQueryAsync();
                    }

                    // Confirmar el cambio de precio
                    using var commit = conn.CreateCommand();
                    commit.CommandText = "COMMIT";
                    await commit.ExecuteNonQueryAsync();
                }
                catch { /* best-effort: no bloquear la orden si falla la corrección de precio */ }
            }
        }

        /// <summary>
        /// Garantiza que cada producto del carrito tenga al menos una fila en ALP_EXISTENCIA.
        /// Si no existe, crea una con cantidad 0 usando la primera bodega disponible.
        /// El SP de conversión hace SELECT INTO en esta tabla; si no hay fila → ORA-01403.
        /// </summary>
        private async Task EnsureExistenciasAsync(OracleConnection conn, List<(int ProductoId, int Cantidad)> items)
        {
            if (items.Count == 0) return;

            // Obtener primer BOD_BODEGA disponible
            int bodegaId = 1;
            try
            {
                using var cmdBod = conn.CreateCommand();
                cmdBod.CommandText = "SELECT BOD_BODEGA FROM ALP_BODEGA WHERE ROWNUM = 1";
                var b = await cmdBod.ExecuteScalarAsync();
                if (b != null && b != DBNull.Value) bodegaId = Convert.ToInt32(b);
            }
            catch { }

            foreach (var (productoId, _) in items)
            {
                try
                {
                    // ¿Ya tiene al menos una fila de existencia?
                    using var cmdCheck = conn.CreateCommand();
                    cmdCheck.CommandText = "SELECT COUNT(1) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = :pId AND ROWNUM = 1";
                    cmdCheck.Parameters.Add("pId", OracleDbType.Int32, productoId, ParameterDirection.Input);
                    var cnt = Convert.ToInt32(await cmdCheck.ExecuteScalarAsync() ?? 0);
                    if (cnt > 0) continue;

                    // Sin fila → crear registro con cantidad 0 en la primera bodega
                    using var cmdIns = conn.CreateCommand();
                    cmdIns.CommandText = @"INSERT INTO ALP_EXISTENCIA
                        (PRO_PRODUCTO, BOD_BODEGA, EXI_CANTIDAD_DISPONIBLE, EXI_CANTIDAD_RESERVADA, EXI_ULTIMA_ACTUALIZACION)
                        VALUES (:pId, :bodId, 0, 0, CURRENT_TIMESTAMP)";
                    cmdIns.Parameters.Add("pId",   OracleDbType.Int32, productoId, ParameterDirection.Input);
                    cmdIns.Parameters.Add("bodId", OracleDbType.Int32, bodegaId,   ParameterDirection.Input);
                    await cmdIns.ExecuteNonQueryAsync();

                    using var commit = conn.CreateCommand();
                    commit.CommandText = "COMMIT";
                    await commit.ExecuteNonQueryAsync();
                }
                catch { /* best-effort */ }
            }
        }

        /// <summary>
        /// Obtiene los items del carrito via SQL directo (sin RefCursor) antes de que el SP los elimine.
        /// </summary>
        private async Task<List<(int ProductoId, int Cantidad)>> ObtenerItemsCarritoDirectoAsync(
            OracleConnection conn, int carritoId)
        {
            var items = new List<(int, int)>();
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT PRO_PRODUCTO, CAD_CANTIDAD
                                    FROM ALP_CARRITO_DETALLE
                                    WHERE CAR_CARRITO = :carritoId";
                cmd.Parameters.Add("carritoId", OracleDbType.Int32, carritoId, ParameterDirection.Input);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    items.Add((reader.GetInt32(0), reader.GetInt32(1)));
            }
            catch { /* best-effort */ }
            return items;
        }

        /// <summary>
        /// Descuenta la cantidad vendida de ALP_EXISTENCIA (la bodega con más stock primero).
        /// Se hace DESPUÉS del COMMIT del SP — es best-effort: si falla, la orden no se cancela.
        /// </summary>
        private async Task DescontarInventarioAsync(
            OracleConnection conn, List<(int ProductoId, int Cantidad)> items)
        {
            try
            {
                foreach (var (productoId, cantidad) in items)
                {
                    using var cmd = conn.CreateCommand();
                    // Descuenta del registro con mayor disponibilidad; usa GREATEST para no ir negativo
                    cmd.CommandText = @"
                        UPDATE ALP_EXISTENCIA
                        SET    EXI_CANTIDAD_DISPONIBLE  = GREATEST(0, EXI_CANTIDAD_DISPONIBLE - :cantidad),
                               EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
                        WHERE  EXI_EXISTENCIA = (
                                   SELECT EXI_EXISTENCIA
                                   FROM   ALP_EXISTENCIA
                                   WHERE  PRO_PRODUCTO = :productoId
                                   ORDER BY EXI_CANTIDAD_DISPONIBLE DESC
                                   FETCH FIRST 1 ROWS ONLY
                               )";
                    cmd.Parameters.Add("cantidad",    OracleDbType.Int32, cantidad,    ParameterDirection.Input);
                    cmd.Parameters.Add("productoId",  OracleDbType.Int32, productoId,  ParameterDirection.Input);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Commit de los descuentos de inventario
                using var commitCmd = conn.CreateCommand();
                commitCmd.CommandText = "COMMIT";
                await commitCmd.ExecuteNonQueryAsync();
            }
            catch { /* best-effort: la orden ya está confirmada */ }
        }

        /// <summary>
        /// Ejecuta SP_OBTENER_CARRITO_CLIENTE que devuelve 2 SYS_REFCURSOR:
        /// p_cur_cabecera (1 fila) y p_cur_detalle (N filas).
        /// Este SP NO tiene p_resultado/p_mensaje; se construye la respuesta manualmente.
        /// </summary>
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

