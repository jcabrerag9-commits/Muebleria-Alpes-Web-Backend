using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Envios;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace MuebleriaAlpesWebBackend.Data.Repositories.Envios
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

            // 1) Actualizar el estado del envío
            const string sqlEnvio = @"
                UPDATE ALP_ENVIO
                   SET ENV_ESTADO = :p_env_estado
                 WHERE ENV_ENVIO  = :p_env_envio";

            using var cmdEnvio = new OracleCommand(sqlEnvio, connection)
            {
                CommandType = CommandType.Text,
                BindByName  = true
            };
            cmdEnvio.Parameters.Add("p_env_estado", OracleDbType.Varchar2).Value = request.Estado;
            cmdEnvio.Parameters.Add("p_env_envio",  OracleDbType.Int32).Value    = request.EnvioId;
            await cmdEnvio.ExecuteNonQueryAsync();

            // 2) Sincronizar el estado de la orden según el estado del envío
            //    EN_TRANSITO / EN_REPARTO → ESO_ESTADO_ORDEN = 3 (Enviado)
            //    ENTREGADO               → ESO_ESTADO_ORDEN = 4 (Entregado)
            var nuevoEstadoOrden = request.Estado.ToUpperInvariant() switch
            {
                "EN_TRANSITO" or "EN_REPARTO" => (int?)3,
                "ENTREGADO"                   => (int?)4,
                _                             => (int?)null
            };

            if (nuevoEstadoOrden.HasValue)
            {
                const string sqlOrden = @"
                    UPDATE ALP_ORDEN_VENTA
                       SET ESO_ESTADO_ORDEN = :p_nuevo_estado
                     WHERE VEN_ORDEN_VENTA  = (
                               SELECT VEN_ORDEN_VENTA FROM ALP_ENVIO
                                WHERE ENV_ENVIO = :p_env_envio2
                           )";

                using var cmdOrden = new OracleCommand(sqlOrden, connection)
                {
                    CommandType = CommandType.Text,
                    BindByName  = true
                };
                cmdOrden.Parameters.Add("p_nuevo_estado", OracleDbType.Int32).Value = nuevoEstadoOrden.Value;
                cmdOrden.Parameters.Add("p_env_envio2",   OracleDbType.Int32).Value = request.EnvioId;
                await cmdOrden.ExecuteNonQueryAsync();
            }

            return true;
        }

        public async Task<bool> ConfirmarEntregaEnvioAsync(ConfirmarEntregaEnvioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            // 1) Marcar envío como ENTREGADO con fecha real
            const string sqlEnvio = @"
                UPDATE ALP_ENVIO
                   SET ENV_ESTADO              = 'ENTREGADO',
                       ENV_FECHA_ENTREGA_REAL  = NVL(:p_fecha, CURRENT_TIMESTAMP),
                       ENV_OBSERVACIONES       = CASE
                                                     WHEN :p_obs IS NULL            THEN ENV_OBSERVACIONES
                                                     WHEN ENV_OBSERVACIONES IS NULL  THEN :p_obs
                                                     ELSE ENV_OBSERVACIONES || ' | ' || :p_obs
                                                 END
                 WHERE ENV_ENVIO = :p_env_envio";

            using var cmdEnvio = new OracleCommand(sqlEnvio, connection)
            {
                CommandType = CommandType.Text,
                BindByName  = true
            };
            cmdEnvio.Parameters.Add("p_fecha",    OracleDbType.TimeStamp).Value = ValorOdbc(request.FechaEntregaReal);
            cmdEnvio.Parameters.Add("p_obs",       OracleDbType.Varchar2).Value  = ValorOdbc(request.Observaciones);
            cmdEnvio.Parameters.Add("p_env_envio", OracleDbType.Int32).Value     = request.EnvioId;
            await cmdEnvio.ExecuteNonQueryAsync();

            // 2) Actualizar la orden a Entregado (ESO_ESTADO_ORDEN = 4)
            const string sqlOrden = @"
                UPDATE ALP_ORDEN_VENTA
                   SET ESO_ESTADO_ORDEN = 4
                 WHERE VEN_ORDEN_VENTA  = (
                           SELECT VEN_ORDEN_VENTA FROM ALP_ENVIO
                            WHERE ENV_ENVIO = :p_env_envio2
                       )";

            using var cmdOrden = new OracleCommand(sqlOrden, connection)
            {
                CommandType = CommandType.Text,
                BindByName  = true
            };
            cmdOrden.Parameters.Add("p_env_envio2", OracleDbType.Int32).Value = request.EnvioId;
            await cmdOrden.ExecuteNonQueryAsync();

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

            const string sql = @"
                SELECT e.ENV_ENVIO,
                       e.VEN_ORDEN_VENTA,
                       e.ENV_NUMERO_GUIA,
                       e.ENV_TRANSPORTISTA,
                       e.ENV_COSTO_ENVIO,
                       e.ENV_FECHA_ENVIO,
                       e.ENV_FECHA_ENTREGA_ESTIMADA,
                       e.ENV_FECHA_ENTREGA_REAL,
                       e.ENV_ESTADO
                  FROM ALP_ENVIO e
                 WHERE e.VEN_ORDEN_VENTA = :p_ven_orden_venta
                 ORDER BY e.ENV_ENVIO DESC";

            using var command = new OracleCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                BindByName = true
            };
            command.Parameters.Add("p_ven_orden_venta", OracleDbType.Int32).Value = ordenVentaId;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearEnvioResumen((OracleDataReader)reader));
            }

            return lista;
        }

        public async Task<List<EnvioResumenResponse>> ListarEnviosPorEstadoAsync(string estado)
        {
            var lista = new List<EnvioResumenResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT e.ENV_ENVIO,
                       e.VEN_ORDEN_VENTA,
                       e.ENV_NUMERO_GUIA,
                       e.ENV_TRANSPORTISTA,
                       e.ENV_COSTO_ENVIO,
                       e.ENV_FECHA_ENVIO,
                       e.ENV_FECHA_ENTREGA_ESTIMADA,
                       e.ENV_FECHA_ENTREGA_REAL,
                       e.ENV_ESTADO
                  FROM ALP_ENVIO e
                 WHERE e.ENV_ESTADO = :p_env_estado
                 ORDER BY e.ENV_ENVIO DESC";

            using var command = new OracleCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                BindByName = true
            };
            command.Parameters.Add("p_env_estado", OracleDbType.Varchar2).Value = estado;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearEnvioResumen((OracleDataReader)reader));
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


        public async Task<List<FiltroOrdenDisponibleResponse>> ListarOrdenesDisponiblesFiltroAsync()
        {
            var resultado = new List<FiltroOrdenDisponibleResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT v.VEN_ORDEN_VENTA,
                       v.VEN_NUMERO_ORDEN,
                       v.CLI_CLIENTE,
                       TRIM(c.CLI_PRIMER_NOMBRE || ' ' || NVL(c.CLI_SEGUNDO_NOMBRE, '') || ' ' || c.CLI_PRIMER_APELLIDO || ' ' || NVL(c.CLI_SEGUNDO_APELLIDO, '')) AS CLIENTE_NOMBRE,
                       v.CLD_CLIENTE_DIRECCION,
                       v.CVE_CANAL_VENTA,
                       cv.CVE_NOMBRE AS CANAL_VENTA,
                       v.VEN_TOTAL,
                       eo.ESO_NOMBRE AS ESTADO_ORDEN,
                       v.VEN_FECHA_ORDEN
                  FROM ALP_ORDEN_VENTA v
                  JOIN ALP_CLIENTE c
                    ON c.CLI_CLIENTE = v.CLI_CLIENTE
                  JOIN ALP_ESTADO_ORDEN eo
                    ON eo.ESO_ESTADO_ORDEN = v.ESO_ESTADO_ORDEN
                  LEFT JOIN ALP_CANAL_VENTA cv
                    ON cv.CVE_CANAL_VENTA = v.CVE_CANAL_VENTA
                 WHERE v.ESO_ESTADO_ORDEN IN (2, 3)
                   AND NOT EXISTS (
                        SELECT 1
                          FROM ALP_ENVIO e
                         WHERE e.VEN_ORDEN_VENTA = v.VEN_ORDEN_VENTA
                           AND e.ENV_ESTADO <> 'FALLIDO'
                   )
                 ORDER BY v.VEN_ORDEN_VENTA DESC";

            using var command = new OracleCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                BindByName = true
            };

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new FiltroOrdenDisponibleResponse
                {
                    OrdenVentaId = ObtenerInt(reader, "VEN_ORDEN_VENTA"),
                    NumeroOrden = ObtenerString(reader, "VEN_NUMERO_ORDEN"),
                    ClienteId = ObtenerInt(reader, "CLI_CLIENTE"),
                    ClienteNombre = ObtenerString(reader, "CLIENTE_NOMBRE"),
                    ClienteDireccionId = ObtenerIntNullable(reader, "CLD_CLIENTE_DIRECCION"),
                    CanalVentaId = ObtenerInt(reader, "CVE_CANAL_VENTA"),
                    CanalVenta = ObtenerString(reader, "CANAL_VENTA"),
                    Total = ObtenerDecimal(reader, "VEN_TOTAL"),
                    EstadoOrden = ObtenerString(reader, "ESTADO_ORDEN"),
                    FechaOrden = ObtenerDateTimeNullable(reader, "VEN_FECHA_ORDEN")
                });
            }

            return resultado;
        }

        public async Task<List<FiltroDireccionClienteResponse>> ListarDireccionesClienteFiltroAsync(int clienteId)
        {
            var resultado = new List<FiltroDireccionClienteResponse>();

            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT d.CLD_CLIENTE_DIRECCION,
                       d.CLI_CLIENTE,
                       d.CIU_CIUDAD,
                       c.CIU_NOMBRE,
                       d.CLD_TIPO,
                       d.CLD_DIRECCION_LINEA1,
                       d.CLD_DIRECCION_LINEA2,
                       d.CLD_CODIGO_POSTAL,
                       d.CLD_REFERENCIA,
                       d.CLD_PRINCIPAL,
                       d.CLD_ESTADO
                  FROM ALP_CLIENTE_DIRECCION d
                  JOIN ALP_CIUDAD c
                    ON c.CIU_CIUDAD = d.CIU_CIUDAD
                 WHERE d.CLI_CLIENTE = :p_cli_cliente
                   AND d.CLD_ESTADO = 'ACTIVO'
                 ORDER BY d.CLD_PRINCIPAL DESC, d.CLD_CLIENTE_DIRECCION DESC";

            using var command = new OracleCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                BindByName = true
            };
            command.Parameters.Add("p_cli_cliente", OracleDbType.Int32).Value = clienteId;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new FiltroDireccionClienteResponse
                {
                    ClienteDireccionId = ObtenerInt(reader, "CLD_CLIENTE_DIRECCION"),
                    ClienteId = ObtenerInt(reader, "CLI_CLIENTE"),
                    CiudadId = ObtenerInt(reader, "CIU_CIUDAD"),
                    CiudadNombre = ObtenerString(reader, "CIU_NOMBRE"),
                    Tipo = ObtenerString(reader, "CLD_TIPO"),
                    DireccionLinea1 = ObtenerString(reader, "CLD_DIRECCION_LINEA1"),
                    DireccionLinea2 = ObtenerString(reader, "CLD_DIRECCION_LINEA2"),
                    CodigoPostal = ObtenerString(reader, "CLD_CODIGO_POSTAL"),
                    Referencia = ObtenerString(reader, "CLD_REFERENCIA"),
                    Principal = ObtenerString(reader, "CLD_PRINCIPAL"),
                    Estado = ObtenerString(reader, "CLD_ESTADO")
                });
            }

            return resultado;
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

        private static int? ObtenerIntNullable(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static decimal ObtenerDecimal(OracleDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0m : Convert.ToDecimal(reader.GetValue(ordinal));
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