using System.Data;
using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Auth;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public AuthRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LoginResponseDto?> LoginAsync(string username, string password)
        {
            using var connection = _connectionFactory.CreateConnection();

            // LEFT JOIN: el usuario se devuelve aunque no tenga rol asignado.
            // También busca el CLI_CLIENTE asociado al username (documento = username en registros web).
            const string sql = @"
                SELECT u.USU_USUARIO                        AS Id,
                       u.USU_USERNAME                       AS Username,
                       u.USU_USERNAME                       AS NombreCompleto,
                       NVL(r.ROL_NOMBRE, 'Administrador')   AS Rol,
                       u.USU_PASSWORD_HASH                  AS PasswordHash,
                       c.CLI_CLIENTE                        AS ClienteId
                FROM   ALP_USUARIO u
                LEFT JOIN ALP_USUARIO_ROL ur
                    ON ur.USU_USUARIO = u.USU_USUARIO
                   AND ur.USR_ESTADO  = 'ACTIVO'
                   AND (ur.USR_FECHA_FIN IS NULL OR ur.USR_FECHA_FIN > CURRENT_TIMESTAMP)
                LEFT JOIN ALP_ROL r
                    ON r.ROL_ROL = ur.ROL_ROL
                LEFT JOIN ALP_CLIENTE c
                    ON UPPER(c.CLI_NUMERO_DOCUMENTO) = UPPER(u.USU_USERNAME)
                   AND c.CLI_ESTADO = 'ACTIVO'
                WHERE  u.USU_USERNAME = :username
                  AND  u.USU_ESTADO   = 'ACTIVO'
                  AND  ROWNUM = 1";

            var interno = await connection.QueryFirstOrDefaultAsync<UsuarioInterno>(
                sql, new { username });

            if (interno == null)
                return null;

            var hashLimpio = interno.PasswordHash.Trim();
            if (!BCrypt.Net.BCrypt.Verify(password, hashLimpio))
                return null;

            return new LoginResponseDto
            {
                Id             = interno.Id,
                Username       = interno.Username,
                NombreCompleto = interno.NombreCompleto,
                Rol            = interno.Rol,
                ClienteId      = interno.ClienteId
            };
        }

        public async Task<AuthDiagnosticoDto> DiagnosticoAsync(string username, string passwordPrueba)
        {
            using var connection = _connectionFactory.CreateConnection();

            var dto = new AuthDiagnosticoDto { Username = username };

            // 1. Verificar si el usuario existe
            const string sqlUser = @"
                SELECT USU_USUARIO       AS Id,
                       USU_USERNAME      AS Username,
                       USU_ESTADO        AS Estado,
                       USU_PASSWORD_HASH AS PasswordHash
                FROM   ALP_USUARIO
                WHERE  USU_USERNAME = :username
                FETCH FIRST 1 ROWS ONLY";

            var user = await connection.QueryFirstOrDefaultAsync<UsuarioInterno>(
                sqlUser, new { username });

            if (user == null)
            {
                dto.UsuarioEncontrado = false;
                dto.Mensaje = "Usuario NO existe en ALP_USUARIO con ese username.";
                return dto;
            }

            dto.UsuarioEncontrado = true;
            dto.UsuarioId         = user.Id;
            dto.EstadoUsuario     = user.Estado;
            var hashDb = user.PasswordHash.Trim();
            dto.HashGuardado = $"{hashDb} ({hashDb.Length} chars, {(hashDb == user.PasswordHash ? "sin espacios extra" : $"TENIA {user.PasswordHash.Length - hashDb.Length} espacios extra!")})";
            dto.HashCompleto = hashDb;

            // 2. Verificar BCrypt con hash limpio
            var hashParaVerif = user.PasswordHash.Trim();
            try
            {
                dto.BCryptOk           = BCrypt.Net.BCrypt.Verify(passwordPrueba, hashParaVerif);
                dto.HashGeneradoPrueba = BCrypt.Net.BCrypt.HashPassword(passwordPrueba, 10);
                // Verificar que el hash recién generado sí funciona con la misma contraseña
                dto.HashNuevoVerifica  = BCrypt.Net.BCrypt.Verify(passwordPrueba, dto.HashGeneradoPrueba);
            }
            catch (Exception ex)
            {
                dto.BCryptOk    = false;
                dto.BCryptError = ex.Message;
            }

            // 3. Verificar si tiene rol asignado
            const string sqlRol = @"
                SELECT r.ROL_NOMBRE
                FROM   ALP_USUARIO_ROL ur
                JOIN   ALP_ROL r ON r.ROL_ROL = ur.ROL_ROL
                WHERE  ur.USU_USUARIO = :usuarioId
                  AND  ur.USR_ESTADO  = 'ACTIVO'
                FETCH FIRST 1 ROWS ONLY";

            var rol = await connection.QueryFirstOrDefaultAsync<string>(
                sqlRol, new { usuarioId = user.Id });

            dto.RolAsignado = rol ?? "(sin rol en ALP_USUARIO_ROL)";

            dto.Mensaje = dto.BCryptOk
                ? "Usuario encontrado y contraseña correcta ✓"
                : $"Usuario encontrado pero BCrypt.Verify falló para password='{passwordPrueba}'.";

            return dto;
        }

        public async Task<(bool Ok, string Mensaje, RegistroResponseDto? Data)> RegistrarAsync(RegistroRequestDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                // 1. Verificar que el username no existe
                const string sqlExiste = @"
                    SELECT COUNT(1) FROM ALP_USUARIO
                    WHERE USU_USERNAME = :username";

                var existe = await connection.ExecuteScalarAsync<int>(sqlExiste, new { username = request.Username });
                if (existe > 0)
                    return (false, "El username ya está en uso", null);

                // 2. Generar USU_CODIGO único
                const string sqlNextNum = @"
                    SELECT NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(USU_CODIGO, '\d+'))), 0) + 1
                    FROM ALP_USUARIO
                    WHERE REGEXP_LIKE(USU_CODIGO, '^USU_\d+$')";

                var nextNum = await connection.ExecuteScalarAsync<int>(sqlNextNum);
                var codigo  = "USU_" + nextNum.ToString("D4");

                // 3. Hash BCrypt
                var hash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 10);

                // 4. INSERT usuario con RETURNING INTO
                var pInsert = new DynamicParameters();
                pInsert.Add("p_codigo",   codigo,           DbType.String);
                pInsert.Add("p_username", request.Username, DbType.String);
                pInsert.Add("p_hash",     hash,             DbType.String);
                pInsert.Add("p_newId",    dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(@"
                    INSERT INTO ALP_USUARIO (USU_CODIGO, USU_USERNAME, USU_PASSWORD_HASH, USU_ESTADO)
                    VALUES (:p_codigo, :p_username, :p_hash, 'ACTIVO')
                    RETURNING USU_USUARIO INTO :p_newId", pInsert);

                int usuarioId = pInsert.Get<int>("p_newId");

                // 5. Buscar rol Cliente
                const string sqlRol = @"
                    SELECT ROL_ROL FROM ALP_ROL
                    WHERE UPPER(ROL_NOMBRE) = 'CLIENTE' AND ROL_ESTADO = 'ACTIVO'
                    FETCH FIRST 1 ROWS ONLY";

                var rolId = await connection.ExecuteScalarAsync<int?>(sqlRol);

                // 6. Crear rol Cliente si no existe
                if (rolId == null)
                {
                    var pRol = new DynamicParameters();
                    pRol.Add("p_rolId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(@"
                        INSERT INTO ALP_ROL (ROL_CODIGO, ROL_NOMBRE, ROL_ESTADO)
                        VALUES ('ROL_CLI', 'Cliente', 'ACTIVO')
                        RETURNING ROL_ROL INTO :p_rolId", pRol);

                    rolId = pRol.Get<int>("p_rolId");
                }

                // 7. Asignar rol al usuario
                await connection.ExecuteAsync(@"
                    INSERT INTO ALP_USUARIO_ROL (USU_USUARIO, ROL_ROL, USR_ESTADO)
                    VALUES (:usuarioId, :rolId, 'ACTIVO')",
                    new { usuarioId, rolId });

                // 8. Crear ALP_CLIENTE automáticamente via SP (PKG_CLIENTES.SP_CREAR_CLIENTE)
                //    EnsureClienteAsync busca primero si ya existe y, si no, lo crea.
                int clienteId = await EnsureClienteAsync(request.Username);

                // 9. Retornar resultado exitoso
                return (true, "Registro exitoso", new RegistroResponseDto
                {
                    Id        = usuarioId,
                    Username  = request.Username,
                    Rol       = "Cliente",
                    ClienteId = clienteId
                });
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<int> EnsureClienteAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();

            // 1. Buscar si ya existe un ALP_CLIENTE con CLI_NUMERO_DOCUMENTO = username
            var existente = await connection.ExecuteScalarAsync<int?>(
                "SELECT CLI_CLIENTE FROM ALP_CLIENTE WHERE UPPER(CLI_NUMERO_DOCUMENTO) = UPPER(:username) AND ROWNUM = 1",
                new { username });

            if (existente.HasValue && existente.Value > 0)
                return existente.Value;

            // 2. No existe → crearlo usando PKG_CLIENTES.SP_CREAR_CLIENTE
            var tipoClienteId   = await connection.ExecuteScalarAsync<int?>("SELECT TCL_TIPO_CLIENTE FROM ALP_TIPO_CLIENTE WHERE ROWNUM = 1");
            var tipoDocumentoId = await connection.ExecuteScalarAsync<int?>("SELECT TDO_TIPO_DOCUMENTO FROM ALP_TIPO_DOCUMENTO WHERE ROWNUM = 1");

            if (!tipoClienteId.HasValue || !tipoDocumentoId.HasValue)
                return 0;

            var p = new DynamicParameters();
            p.Add("p_tcl_tipo_cliente",      tipoClienteId.Value,  DbType.Int32);
            p.Add("p_tdo_tipo_documento",     tipoDocumentoId.Value, DbType.Int32);
            p.Add("p_cli_numero_documento",   username,             DbType.String);
            p.Add("p_cli_primer_nombre",      username,             DbType.String);
            p.Add("p_cli_segundo_nombre",     null,                 DbType.String);
            p.Add("p_cli_primer_apellido",    "Web",                DbType.String);
            p.Add("p_cli_segundo_apellido",   null,                 DbType.String);
            p.Add("p_cli_razon_social",       null,                 DbType.String);
            p.Add("p_cli_fecha_nacimiento",   null,                 DbType.DateTime);
            p.Add("p_cli_genero",             "O",                  DbType.String);
            p.Add("p_cli_password_plano",     null,                 DbType.String);
            p.Add("p_usuario_id",             1,                    DbType.Int32);
            p.Add("p_resultado", dbType: DbType.String,  direction: ParameterDirection.Output, size: 50);
            p.Add("p_mensaje",   dbType: DbType.String,  direction: ParameterDirection.Output, size: 255);
            p.Add("p_id",        dbType: DbType.Int32,   direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_CREAR_CLIENTE", p, commandType: CommandType.StoredProcedure);

            var resultado = p.Get<string>("p_resultado");
            if (resultado == "EXITO")
                return p.Get<int>("p_id");

            return 0;
        }

        private sealed class UsuarioInterno
        {
            public int    Id             { get; set; }
            public string Username       { get; set; } = string.Empty;
            public string Estado         { get; set; } = string.Empty;
            public string NombreCompleto { get; set; } = string.Empty;
            public string Rol            { get; set; } = string.Empty;
            public string PasswordHash   { get; set; } = string.Empty;
            public int    ClienteId      { get; set; }   // CLI_CLIENTE, 0 si no hay
        }
    }
}
