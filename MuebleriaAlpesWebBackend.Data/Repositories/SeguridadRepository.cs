using System.Data;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Seguridad;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class SeguridadRepository : ISeguridadRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public SeguridadRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CrearUsuarioResponse> CrearUsuarioAsync(CrearUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_CREAR_USUARIO");

            command.Parameters.Add("p_usu_username", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add("p_usu_password_plano", OracleDbType.Varchar2).Value = request.PasswordPlano;
            command.Parameters.Add("p_usu_estado", OracleDbType.Varchar2).Value = request.Estado;

            var usuarioOut = new OracleParameter("p_usu_usuario_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(usuarioOut);

            var codigoOut = new OracleParameter("p_usu_codigo_out", OracleDbType.Varchar2, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(codigoOut);

            await command.ExecuteNonQueryAsync();

            return new CrearUsuarioResponse
            {
                UsuarioId = Convert.ToInt32(usuarioOut.Value.ToString()),
                Codigo = codigoOut.Value?.ToString()
            };
        }

        public async Task<bool> ActualizarUsuarioAsync(ActualizarUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ACTUALIZAR_USUARIO");

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;
            command.Parameters.Add("p_usu_username", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add("p_usu_estado", OracleDbType.Varchar2).Value = request.Estado;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> CambiarPasswordUsuarioAsync(CambiarPasswordUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_CAMBIAR_PASSWORD_USUARIO");

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;
            command.Parameters.Add("p_password_plano", OracleDbType.Varchar2).Value = request.PasswordPlano;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> BloquearUsuarioAsync(BloquearUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_BLOQUEAR_USUARIO");
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> DesbloquearUsuarioAsync(DesbloquearUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_DESBLOQUEAR_USUARIO");
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> EliminarUsuarioLogicoAsync(EliminarUsuarioLogicoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ELIMINAR_USUARIO_LOGICO");
            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<CrearRolResponse> CrearRolAsync(CrearRolRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_CREAR_ROL");

            command.Parameters.Add("p_rol_codigo", OracleDbType.Varchar2).Value = request.Codigo;
            command.Parameters.Add("p_rol_nombre", OracleDbType.Varchar2).Value = request.Nombre;
            command.Parameters.Add("p_rol_descripcion", OracleDbType.Varchar2).Value = ValorDb(request.Descripcion);
            command.Parameters.Add("p_rol_estado", OracleDbType.Varchar2).Value = request.Estado;

            var rolOut = new OracleParameter("p_rol_rol_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(rolOut);

            await command.ExecuteNonQueryAsync();

            return new CrearRolResponse
            {
                RolId = Convert.ToInt32(rolOut.Value.ToString())
            };
        }

        public async Task<bool> ActualizarRolAsync(ActualizarRolRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ACTUALIZAR_ROL");

            command.Parameters.Add("p_rol_rol", OracleDbType.Int32).Value = request.RolId;
            command.Parameters.Add("p_rol_codigo", OracleDbType.Varchar2).Value = request.Codigo;
            command.Parameters.Add("p_rol_nombre", OracleDbType.Varchar2).Value = request.Nombre;
            command.Parameters.Add("p_rol_descripcion", OracleDbType.Varchar2).Value = ValorDb(request.Descripcion);
            command.Parameters.Add("p_rol_estado", OracleDbType.Varchar2).Value = request.Estado;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> EliminarRolLogicoAsync(EliminarRolLogicoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ELIMINAR_ROL_LOGICO");
            command.Parameters.Add("p_rol_rol", OracleDbType.Int32).Value = request.RolId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<CrearPermisoResponse> CrearPermisoAsync(CrearPermisoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_CREAR_PERMISO");

            command.Parameters.Add("p_per_codigo", OracleDbType.Varchar2).Value = request.Codigo;
            command.Parameters.Add("p_per_nombre", OracleDbType.Varchar2).Value = request.Nombre;
            command.Parameters.Add("p_per_descripcion", OracleDbType.Varchar2).Value = ValorDb(request.Descripcion);
            command.Parameters.Add("p_per_estado", OracleDbType.Varchar2).Value = request.Estado;

            var permisoOut = new OracleParameter("p_per_permiso_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(permisoOut);

            await command.ExecuteNonQueryAsync();

            return new CrearPermisoResponse
            {
                PermisoId = Convert.ToInt32(permisoOut.Value.ToString())
            };
        }

        public async Task<bool> ActualizarPermisoAsync(ActualizarPermisoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ACTUALIZAR_PERMISO");

            command.Parameters.Add("p_per_permiso", OracleDbType.Int32).Value = request.PermisoId;
            command.Parameters.Add("p_per_codigo", OracleDbType.Varchar2).Value = request.Codigo;
            command.Parameters.Add("p_per_nombre", OracleDbType.Varchar2).Value = request.Nombre;
            command.Parameters.Add("p_per_descripcion", OracleDbType.Varchar2).Value = ValorDb(request.Descripcion);
            command.Parameters.Add("p_per_estado", OracleDbType.Varchar2).Value = request.Estado;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> EliminarPermisoLogicoAsync(EliminarPermisoLogicoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ELIMINAR_PERMISO_LOGICO");
            command.Parameters.Add("p_per_permiso", OracleDbType.Int32).Value = request.PermisoId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<AsignarRolUsuarioResponse> AsignarRolUsuarioAsync(AsignarRolUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ASIGNAR_ROL_USUARIO");

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;
            command.Parameters.Add("p_rol_rol", OracleDbType.Int32).Value = request.RolId;
            command.Parameters.Add("p_usr_fecha_inicio", OracleDbType.TimeStamp).Value = ValorDb(request.FechaInicio);
            command.Parameters.Add("p_usr_fecha_fin", OracleDbType.TimeStamp).Value = ValorDb(request.FechaFin);
            command.Parameters.Add("p_usr_estado", OracleDbType.Varchar2).Value = request.Estado;

            var usuarioRolOut = new OracleParameter("p_usr_usuario_rol_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(usuarioRolOut);

            await command.ExecuteNonQueryAsync();

            return new AsignarRolUsuarioResponse
            {
                UsuarioRolId = Convert.ToInt32(usuarioRolOut.Value.ToString())
            };
        }

        public async Task<bool> QuitarRolUsuarioAsync(QuitarRolUsuarioRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_QUITAR_ROL_USUARIO");
            command.Parameters.Add("p_usr_usuario_rol", OracleDbType.Int32).Value = request.UsuarioRolId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<AsignarPermisoRolResponse> AsignarPermisoRolAsync(AsignarPermisoRolRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_ASIGNAR_PERMISO_ROL");

            command.Parameters.Add("p_rol_rol", OracleDbType.Int32).Value = request.RolId;
            command.Parameters.Add("p_per_permiso", OracleDbType.Int32).Value = request.PermisoId;
            command.Parameters.Add("p_rop_estado", OracleDbType.Varchar2).Value = request.Estado;

            var rolPermisoOut = new OracleParameter("p_rop_rol_permiso_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(rolPermisoOut);

            await command.ExecuteNonQueryAsync();

            return new AsignarPermisoRolResponse
            {
                RolPermisoId = Convert.ToInt32(rolPermisoOut.Value.ToString())
            };
        }

        public async Task<bool> QuitarPermisoRolAsync(QuitarPermisoRolRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_QUITAR_PERMISO_ROL");
            command.Parameters.Add("p_rop_rol_permiso", OracleDbType.Int32).Value = request.RolPermisoId;

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<RegistrarBitacoraAccesoResponse> RegistrarBitacoraAccesoAsync(RegistrarBitacoraAccesoRequest request)
        {
            using var connection = (OracleConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = CrearComando(connection, "PKG_SEGURIDAD.SP_REGISTRAR_BITACORA_ACCESO");

            command.Parameters.Add("p_usu_usuario", OracleDbType.Int32).Value = request.UsuarioId;
            command.Parameters.Add("p_bia_username", OracleDbType.Varchar2).Value = request.Username;
            command.Parameters.Add("p_bia_ip", OracleDbType.Varchar2).Value = ValorDb(request.Ip);
            command.Parameters.Add("p_bia_user_agent", OracleDbType.Varchar2).Value = ValorDb(request.UserAgent);
            command.Parameters.Add("p_bia_resultado", OracleDbType.Varchar2).Value = request.Resultado;
            command.Parameters.Add("p_bia_detalle", OracleDbType.Varchar2).Value = ValorDb(request.Detalle);

            var bitacoraOut = new OracleParameter("p_bia_bitacora_acceso_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(bitacoraOut);

            await command.ExecuteNonQueryAsync();

            return new RegistrarBitacoraAccesoResponse
            {
                BitacoraAccesoId = Convert.ToInt32(bitacoraOut.Value.ToString())
            };
        }

        private static OracleCommand CrearComando(OracleConnection connection, string procedureName)
        {
            return new OracleCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static object ValorDb(object? value)
        {
            return value ?? DBNull.Value;
        }
    }
}