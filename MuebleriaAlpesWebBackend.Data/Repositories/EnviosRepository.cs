using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class EnviosRepository : IEnviosRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public EnviosRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> RegistrarEnvioAsync(RegistrarEnvioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_REGISTRAR_ENVIO");

            command.Parameters.Add("p_ven_orden_venta", OracleDbType.Int32).Value = request.OrdenVentaId;
            command.Parameters.Add("p_cld_cliente_direccion", OracleDbType.Int32).Value = request.ClienteDireccionId;
            command.Parameters.Add("p_env_numero_guia", OracleDbType.Varchar2).Value = ValorOdbc(request.NumeroGuia);
            command.Parameters.Add("p_env_transportista", OracleDbType.Varchar2).Value = ValorOdbc(request.Transportista);
            command.Parameters.Add("p_env_costo_envio", OracleDbType.Decimal).Value = ValorOdbc(request.CostoEnvio);
            command.Parameters.Add("p_env_fecha_envio", OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEnvio);
            command.Parameters.Add("p_env_fecha_entrega_estimada", OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEntregaEstimada);
            command.Parameters.Add("p_env_estado", OracleDbType.Varchar2).Value = ValorOdbc(request.Estado);
            command.Parameters.Add("p_env_observaciones", OracleDbType.Varchar2).Value = ValorOdbc(request.Observaciones);

            var outputParam = new OracleParameter("p_env_envio_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            await command.ExecuteNonQueryAsync();

            return Convert.ToInt32(outputParam.Value.ToString());
        }

        public async Task<bool> ActualizarEnvioAsync(ActualizarEnvioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_ACTUALIZAR_ENVIO");

            command.Parameters.Add("p_env_envio", OracleDbType.Int32).Value = request.EnvioId;
            command.Parameters.Add("p_cld_cliente_direccion", OracleDbType.Int32).Value = request.ClienteDireccionId;
            command.Parameters.Add("p_env_numero_guia", OracleDbType.Varchar2).Value = ValorOdbc(request.NumeroGuia);
            command.Parameters.Add("p_env_transportista", OracleDbType.Varchar2).Value = ValorOdbc(request.Transportista);
            command.Parameters.Add("p_env_costo_envio", OracleDbType.Decimal).Value = ValorOdbc(request.CostoEnvio);
            command.Parameters.Add("p_env_fecha_envio", OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEnvio);
            command.Parameters.Add("p_env_fecha_entrega_estimada", OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEntregaEstimada);
            command.Parameters.Add("p_env_observaciones", OracleDbType.Varchar2).Value = ValorOdbc(request.Observaciones);

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> CambiarEstadoEnvioAsync(CambiarEstadoEnvioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_CAMBIAR_ESTADO_ENVIO");

            command.Parameters.Add("p_env_envio", OracleDbType.Int32).Value = request.EnvioId;
            command.Parameters.Add("p_env_estado", OracleDbType.Varchar2).Value = request.Estado;

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> ConfirmarEntregaEnvioAsync(ConfirmarEntregaEnvioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_CONFIRMAR_ENTREGA_ENVIO");

            command.Parameters.Add("p_env_envio", OracleDbType.Int32).Value = request.EnvioId;
            command.Parameters.Add("p_env_fecha_entrega_real", OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEntregaReal);
            command.Parameters.Add("p_env_observaciones", OracleDbType.Varchar2).Value = ValorOdbc(request.Observaciones);

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> MarcarOrdenEnviadaAsync(int ordenVentaId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_MARCAR_ORDEN_ENVIADA");

            command.Parameters.Add("p_ven_orden_venta", OracleDbType.Int32).Value = ordenVentaId;

            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<EnvioResponse?> ObtenerEnvioAsync(int envioId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_OBTENER_ENVIO");

            command.Parameters.Add("p_env_envio", OracleDbType.Int32).Value = envioId;

            var cursorParam = new OracleParameter("p_resultado", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(cursorParam);

            await command.ExecuteNonQueryAsync();

            using var refCursor = (OracleRefCursor)cursorParam.Value;
            using var reader = refCursor.GetDataReader();

            if (reader.Read())
            {
                return MapearEnvio(reader);
            }

            return null;
        }

        public async Task<List<EnvioResumenResponse>> ListarEnviosPorOrdenAsync(int ordenVentaId)
        {
            var lista = new List<EnvioResumenResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_LISTAR_ENVIOS_POR_ORDEN");

            command.Parameters.Add("p_ven_orden_venta", OracleDbType.Int32).Value = ordenVentaId;

            var cursorParam = new OracleParameter("p_resultado", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(cursorParam);

            await command.ExecuteNonQueryAsync();

            using var refCursor = (OracleRefCursor)cursorParam.Value;
            using var reader = refCursor.GetDataReader();

            while (reader.Read())
            {
                lista.Add(MapearEnvioResumen(reader));
            }

            return lista;
        }

        public async Task<List<EnvioResumenResponse>> ListarEnviosPorEstadoAsync(string estado)
        {
            var lista = new List<EnvioResumenResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_LISTAR_ENVIOS_POR_ESTADO");

            command.Parameters.Add("p_env_estado", OracleDbType.Varchar2).Value = estado;

            var cursorParam = new OracleParameter("p_resultado", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(cursorParam);

            await command.ExecuteNonQueryAsync();

            using var refCursor = (OracleRefCursor)cursorParam.Value;
            using var reader = refCursor.GetDataReader();

            while (reader.Read())
            {
                lista.Add(MapearEnvioResumen(reader));
            }

            return lista;
        }

        public async Task<EnvioResponse?> BuscarEnvioPorGuiaAsync(string numeroGuia)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_ENVIOS.SP_BUSCAR_ENVIO_POR_GUIA");

            command.Parameters.Add("p_env_numero_guia", OracleDbType.Varchar2).Value = numeroGuia;

            var cursorParam = new OracleParameter("p_resultado", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(cursorParam);

            await command.ExecuteNonQueryAsync();

            using var refCursor = (OracleRefCursor)cursorParam.Value;
            using var reader = refCursor.GetDataReader();

            if (reader.Read())
            {
                return MapearEnvio(reader);
            }

            return null;
        }

        private static OracleCommand CrearComando(OracleConnection connection, string procedureName)
        {
            return new OracleCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static object ValorOdbc(object? value)
        {
            return value ?? DBNull.Value;
        }

        private static EnvioResponse MapearEnvio(OracleDataReader reader)
        {
            return new EnvioResponse
            {
                EnvioId = ObtenerInt(reader, "ENV_ENVIO"),
                OrdenVentaId = ObtenerInt(reader, "VEN_ORDEN_VENTA"),
                ClienteDireccionId = ObtenerInt(reader, "CLD_CLIENTE_DIRECCION"),
                NumeroGuia = ObtenerString(reader, "ENV_NUMERO_GUIA"),
                Transportista = ObtenerString(reader, "ENV_TRANSPORTISTA"),
                CostoEnvio = ObtenerDecimalNullable(reader, "ENV_COSTO_ENVIO"),
                FechaEnvio = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENVIO"),
                FechaEntregaEstimada = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENTREGA_ESTIMADA"),
                FechaEntregaReal = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENTREGA_REAL"),
                Estado = ObtenerString(reader, "ENV_ESTADO"),
                Observaciones = ObtenerString(reader, "ENV_OBSERVACIONES")
            };
        }

        private static EnvioResumenResponse MapearEnvioResumen(OracleDataReader reader)
        {
            return new EnvioResumenResponse
            {
                EnvioId = ObtenerInt(reader, "ENV_ENVIO"),
                OrdenVentaId = ObtenerInt(reader, "VEN_ORDEN_VENTA"),
                NumeroGuia = ObtenerString(reader, "ENV_NUMERO_GUIA"),
                Transportista = ObtenerString(reader, "ENV_TRANSPORTISTA"),
                CostoEnvio = ObtenerDecimalNullable(reader, "ENV_COSTO_ENVIO"),
                FechaEnvio = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENVIO"),
                FechaEntregaEstimada = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENTREGA_ESTIMADA"),
                FechaEntregaReal = ObtenerDateTimeNullable(reader, "ENV_FECHA_ENTREGA_REAL"),
                Estado = ObtenerString(reader, "ENV_ESTADO")
            };
        }

        private static int ObtenerInt(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static string? ObtenerString(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static decimal? ObtenerDecimalNullable(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : Convert.ToDecimal(reader.GetValue(ordinal));
        }

        private static DateTime? ObtenerDateTimeNullable(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : Convert.ToDateTime(reader.GetValue(ordinal));
        }
    }
}