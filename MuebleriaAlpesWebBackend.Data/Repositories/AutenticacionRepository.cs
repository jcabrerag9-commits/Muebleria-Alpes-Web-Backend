using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Autenticacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class AutenticacionRepository : IAutenticacionRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public AutenticacionRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ValidarLoginResponse> ValidarLoginAsync(ValidarLoginRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_AUTENTICACION.FN_VALIDAR_LOGIN");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Int32)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add("p_password_plano", OracleDbType.Varchar2).Value = request.PasswordPlano;

            await command.ExecuteNonQueryAsync();

            var valor = Convert.ToInt32(returnParam.Value.ToString());

            return new ValidarLoginResponse
            {
                EsValido = valor == 1
            };
        }

        public async Task<IniciarSesionResponse> IniciarSesionAsync(IniciarSesionRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_AUTENTICACION.SP_INICIAR_SESION");

            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add("p_password_plano", OracleDbType.Varchar2).Value = request.PasswordPlano;
            command.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = ValorDb(request.Ip);
            command.Parameters.Add("p_user_agent", OracleDbType.Varchar2).Value = ValorDb(request.UserAgent);

            var usuarioOut = new OracleParameter("p_usu_usuario_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(usuarioOut);

            var sesionOut = new OracleParameter("p_ses_sesion_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(sesionOut);

            var tokenOut = new OracleParameter("p_ses_token_out", OracleDbType.Varchar2, 500)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(tokenOut);

            var esValidoOut = new OracleParameter("p_es_valido_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(esValidoOut);

            await command.ExecuteNonQueryAsync();

            return new IniciarSesionResponse
            {
                UsuarioId = ConvertirIntNullable(usuarioOut.Value),
                SesionId = ConvertirIntNullable(sesionOut.Value),
                TokenSesion = ConvertirStringNullable(tokenOut.Value),
                EsValido = Convert.ToInt32(esValidoOut.Value.ToString()) == 1
            };
        }

        public async Task<bool> CerrarSesionAsync(CerrarSesionRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_AUTENTICACION.SP_CERRAR_SESION");
            command.Parameters.Add("p_ses_token", OracleDbType.Varchar2).Value = request.TokenSesion;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<GenerarTokenRecuperacionResponse> GenerarTokenRecuperacionAsync(GenerarTokenRecuperacionRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_AUTENTICACION.SP_GENERAR_TOKEN_RECUPERACION");

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;
            command.Parameters.Add("p_rcl_fecha_expiracion", OracleDbType.TimeStamp).Value = request.FechaExpiracion;

            var recuperacionOut = new OracleParameter("p_rcl_recuperacion_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(recuperacionOut);

            var tokenOut = new OracleParameter("p_rcl_token_out", OracleDbType.Varchar2, 500)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(tokenOut);

            await command.ExecuteNonQueryAsync();

            return new GenerarTokenRecuperacionResponse
            {
                RecuperacionClaveId = Convert.ToInt32(recuperacionOut.Value.ToString()),
                Token = ConvertirStringNullable(tokenOut.Value)
            };
        }

        public async Task<bool> MarcarTokenRecuperacionUsadoAsync(MarcarTokenRecuperacionUsadoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoProcedimiento(connection, "PKG_AUTENTICACION.SP_MARCAR_TOKEN_RECUPERACION_USADO");
            command.Parameters.Add("p_rcl_token", OracleDbType.Varchar2).Value = request.Token;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<SesionActivaResponse> SesionActivaAsync(int usuarioId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_AUTENTICACION.FN_SESION_ACTIVA");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Int32)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = usuarioId;

            await command.ExecuteNonQueryAsync();

            var valor = Convert.ToInt32(returnParam.Value.ToString());

            return new SesionActivaResponse
            {
                Activa = valor == 1
            };
        }

        public async Task<TokenRecuperacionValidoResponse> TokenRecuperacionValidoAsync(string token)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_AUTENTICACION.FN_TOKEN_RECUPERACION_VALIDO");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Int32)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_rcl_token", OracleDbType.Varchar2).Value = token;

            await command.ExecuteNonQueryAsync();

            var valor = Convert.ToInt32(returnParam.Value.ToString());

            return new TokenRecuperacionValidoResponse
            {
                EsValido = valor == 1
            };
        }

        public async Task<ClientePorUsuarioResponse> ObtenerClientePorUsuarioAsync(int usuarioId)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComandoFuncion(connection, "PKG_AUTENTICACION.FN_OBTENER_CLIENTE_POR_USUARIO");

            var returnParam = new OracleParameter("p_resultado", OracleDbType.Int32)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParam);

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = usuarioId;

            await command.ExecuteNonQueryAsync();

            return new ClientePorUsuarioResponse
            {
                ClienteId = ConvertirIntNullable(returnParam.Value)
            };
        }

        private static OracleCommand CrearComandoProcedimiento(OracleConnection connection, string procedureName)
        {
            return new OracleCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static OracleCommand CrearComandoFuncion(OracleConnection connection, string functionName)
        {
            return new OracleCommand(functionName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static object ValorDb(object? value)
        {
            return value ?? DBNull.Value;
        }

        private static int? ConvertirIntNullable(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var texto = value.ToString();

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (texto.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            return Convert.ToInt32(texto);
        }

        private static string? ConvertirStringNullable(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var texto = value.ToString();

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (texto.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            return texto;
        }
    }
}