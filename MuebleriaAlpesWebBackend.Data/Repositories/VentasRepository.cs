using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Ventas;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class VentasRepository : IVentasRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public VentasRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Ejecuta SP_CREAR_ORDEN_COMPLETA.
        /// SYS.ODCINUMBERLIST (VARRAY) no es soportado directamente por ODP.NET Core como UDT.
        /// Se usa un bloque PL/SQL anónimo que recibe PLSQLAssociativeArray (int[]) y construye
        /// los ODCINUMBERLIST internamente antes de invocar el SP.
        /// </summary>
        public async Task<BaseResponse<CrearOrdenDataDto>> CrearOrdenCompletaAsync(CrearOrdenRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.BindByName = true;

            int[] productosArray = request.ProductosIds.ToArray();
            int[] cantidadesArray = request.Cantidades.ToArray();
            int arraySize = productosArray.Length;

            // Bloque PL/SQL anónimo: recibe arreglos asociativos y construye ODCINUMBERLIST
            cmd.CommandText = @"
                DECLARE
                    v_productos  SYS.ODCINUMBERLIST := SYS.ODCINUMBERLIST();
                    v_cantidades SYS.ODCINUMBERLIST := SYS.ODCINUMBERLIST();
                BEGIN
                    v_productos.EXTEND(:p_array_size);
                    v_cantidades.EXTEND(:p_array_size);
                    FOR i IN 1..:p_array_size LOOP
                        v_productos(i)  := :p_productos_ids(i);
                        v_cantidades(i) := :p_cantidades(i);
                    END LOOP;

                    SP_CREAR_ORDEN_COMPLETA(
                        p_cliente_id    => :p_cliente_id,
                        p_canal_venta   => :p_canal_venta,
                        p_productos_ids => v_productos,
                        p_cantidades    => v_cantidades,
                        p_orden_id      => :p_orden_id,
                        p_total         => :p_total,
                        p_resultado     => :p_resultado,
                        p_mensaje       => :p_mensaje
                    );
                END;";
            cmd.CommandType = CommandType.Text;

            // --- Parámetro de tamaño del arreglo ---
            cmd.Parameters.Add("p_array_size", OracleDbType.Int32, arraySize, ParameterDirection.Input);

            // --- PLSQLAssociativeArray para productos ---
            var pProductos = new OracleParameter("p_productos_ids", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Input,
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                Value = productosArray,
                Size = arraySize
            };
            cmd.Parameters.Add(pProductos);

            // --- PLSQLAssociativeArray para cantidades ---
            var pCantidades = new OracleParameter("p_cantidades", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Input,
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                Value = cantidadesArray,
                Size = arraySize
            };
            cmd.Parameters.Add(pCantidades);

            // --- Parámetros IN escalares ---
            cmd.Parameters.Add("p_cliente_id", OracleDbType.Int32, request.ClienteId, ParameterDirection.Input);
            cmd.Parameters.Add("p_canal_venta", OracleDbType.Int32, request.CanalVenta, ParameterDirection.Input);

            // --- Parámetros OUT ---
            var pOrdenId = new OracleParameter("p_orden_id", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pOrdenId);

            var pTotal = new OracleParameter("p_total", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pTotal);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse<CrearOrdenDataDto>
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty,
                Data = new CrearOrdenDataDto
                {
                    OrdenId = pOrdenId.Value is OracleDecimal dec && !dec.IsNull ? (int?)dec.ToInt32() : null,
                    Total = pTotal.Value is OracleDecimal totalDec && !totalDec.IsNull ? totalDec.Value : 0
                }
            };
        }

        public async Task<BaseResponse> ActualizarEstadoOrdenAsync(ActualizarEstadoOrdenRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_ACTUALIZAR_ESTADO_ORDEN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_orden_id", OracleDbType.Int32, request.OrdenId, ParameterDirection.Input);
            cmd.Parameters.Add("p_nuevo_estado", OracleDbType.Int32, request.NuevoEstado, ParameterDirection.Input);
            cmd.Parameters.Add("p_usuario_id", OracleDbType.Int32, request.UsuarioId, ParameterDirection.Input);
            cmd.Parameters.Add("p_comentario", OracleDbType.Varchar2, request.Comentario, ParameterDirection.Input);

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

        public async Task<BaseResponse> CancelarOrdenAsync(CancelarOrdenRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CANCELAR_ORDEN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_orden_id", OracleDbType.Int32, request.OrdenId, ParameterDirection.Input);
            cmd.Parameters.Add("p_motivo", OracleDbType.Varchar2, request.Motivo, ParameterDirection.Input);
            cmd.Parameters.Add("p_usuario_id", OracleDbType.Int32, request.UsuarioId, ParameterDirection.Input);

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

        public async Task<BaseResponse> AplicarPromocionAsync(AplicarPromocionRequestDto request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_APLICAR_PROMOCION";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_orden_id", OracleDbType.Int32, request.OrdenId, ParameterDirection.Input);
            cmd.Parameters.Add("p_promocion_id", OracleDbType.Int32, request.PromocionId, ParameterDirection.Input);

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

        public async Task<BaseResponse<CalcularTotalesOrdenDataDto>> CalcularTotalesOrdenAsync(int ordenId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SP_CALCULAR_TOTALES_ORDEN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_orden_id", OracleDbType.Int32, ordenId, ParameterDirection.Input);

            var pSubtotal = new OracleParameter("p_subtotal", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pSubtotal);

            var pDescuento = new OracleParameter("p_descuento", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pDescuento);

            var pEnvio = new OracleParameter("p_envio", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pEnvio);

            var pImpuestos = new OracleParameter("p_impuestos", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pImpuestos);

            var pTotal = new OracleParameter("p_total", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(pTotal);

            var pResultado = new OracleParameter("p_resultado", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(pResultado);

            var pMensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            cmd.Parameters.Add(pMensaje);

            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse<CalcularTotalesOrdenDataDto>
            {
                Resultado = pResultado.Value?.ToString() ?? string.Empty,
                Mensaje = pMensaje.Value?.ToString() ?? string.Empty,
                Data = new CalcularTotalesOrdenDataDto
                {
                    Subtotal = pSubtotal.Value is OracleDecimal subDec && !subDec.IsNull ? subDec.Value : 0,
                    Descuento = pDescuento.Value is OracleDecimal descDec && !descDec.IsNull ? descDec.Value : 0,
                    Envio = pEnvio.Value is OracleDecimal envDec && !envDec.IsNull ? envDec.Value : 0,
                    Impuestos = pImpuestos.Value is OracleDecimal impDec && !impDec.IsNull ? impDec.Value : 0,
                    Total = pTotal.Value is OracleDecimal totDec && !totDec.IsNull ? totDec.Value : 0
                }
            };
        }
    }
}
