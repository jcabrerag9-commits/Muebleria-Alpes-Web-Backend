using Dapper;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public ClienteRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #region Maestro
        public async Task<(int id, string codigo)> CrearClienteAsync(Cliente cliente)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_tcl_tipo_cliente", cliente.TipoClienteId);
            parameters.Add("p_tdo_tipo_documento", cliente.TipoDocumentoId);
            parameters.Add("p_cli_numero_documento", cliente.NumeroDocumento);
            parameters.Add("p_cli_primer_nombre", cliente.PrimerNombre);
            parameters.Add("p_cli_segundo_nombre", cliente.SegundoNombre);
            parameters.Add("p_cli_primer_apellido", cliente.PrimerApellido);
            parameters.Add("p_cli_segundo_apellido", cliente.SegundoApellido);
            parameters.Add("p_cli_razon_social", cliente.RazonSocial);
            parameters.Add("p_cli_fecha_nacimiento", cliente.FechaNacimiento);
            parameters.Add("p_cli_genero", cliente.Genero);
            parameters.Add("p_cli_password_plano", cliente.PasswordPlano);
            parameters.Add("p_cli_cliente_out", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_cli_codigo_out", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_CREAR_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
            
            return (parameters.Get<int>("p_cli_cliente_out"), parameters.Get<string>("p_cli_codigo_out"));
        }

        public async Task ActualizarClienteAsync(Cliente cliente)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", cliente.Id);
            parameters.Add("p_tcl_tipo_cliente", cliente.TipoClienteId);
            parameters.Add("p_tdo_tipo_documento", cliente.TipoDocumentoId);
            parameters.Add("p_cli_numero_documento", cliente.NumeroDocumento);
            parameters.Add("p_cli_primer_nombre", cliente.PrimerNombre);
            parameters.Add("p_cli_segundo_nombre", cliente.SegundoNombre);
            parameters.Add("p_cli_primer_apellido", cliente.PrimerApellido);
            parameters.Add("p_cli_segundo_apellido", cliente.SegundoApellido);
            parameters.Add("p_cli_razon_social", cliente.RazonSocial);
            parameters.Add("p_cli_fecha_nacimiento", cliente.FechaNacimiento);
            parameters.Add("p_cli_genero", cliente.Genero);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_ACTUALIZAR_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task CambiarEstadoAsync(int clienteId, string nuevoEstado, string motivo, int? usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", clienteId);
            parameters.Add("p_cli_estado", nuevoEstado);
            parameters.Add("p_ceh_motivo", motivo);
            parameters.Add("p_usu_usuario", usuarioId);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_CAMBIAR_ESTADO_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarLogicoAsync(int clienteId, string motivo, int? usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", clienteId);
            parameters.Add("p_ceh_motivo", motivo);
            parameters.Add("p_usu_usuario", usuarioId);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_ELIMINAR_CLIENTE_LOGICO", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Cliente>> ListarAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT CLI_CLIENTE as Id, TCL_TIPO_CLIENTE as TipoClienteId, TDO_TIPO_DOCUMENTO as TipoDocumentoId, 
                                    CLI_CODIGO as Codigo, CLI_NUMERO_DOCUMENTO as NumeroDocumento, CLI_PRIMER_NOMBRE as PrimerNombre, 
                                    CLI_SEGUNDO_NOMBRE as SegundoNombre, CLI_PRIMER_APELLIDO as PrimerApellido, CLI_SEGUNDO_APELLIDO as SegundoApellido, 
                                    CLI_RAZON_SOCIAL as RazonSocial, CLI_FECHA_NACIMIENTO as FechaNacimiento, CLI_GENERO as Genero, CLI_ESTADO as Estado 
                             FROM ALP_CLIENTE WHERE CLI_ESTADO <> 'ELIMINADO' ORDER BY CLI_PRIMER_APELLIDO, CLI_PRIMER_NOMBRE";
            return await connection.QueryAsync<Cliente>(query);
        }

        public async Task<ClienteDetalleDto> GetDetalleAsync(int clienteId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 1. Maestro
            string qMaestro = @"SELECT c.CLI_CLIENTE as Id, c.TCL_TIPO_CLIENTE as TipoClienteId, c.TDO_TIPO_DOCUMENTO as TipoDocumentoId, 
                                       c.CLI_CODIGO as Codigo, c.CLI_NUMERO_DOCUMENTO as NumeroDocumento, c.CLI_PRIMER_NOMBRE as PrimerNombre, 
                                       c.CLI_SEGUNDO_NOMBRE as SegundoNombre, c.CLI_PRIMER_APELLIDO as PrimerApellido, c.CLI_SEGUNDO_APELLIDO as SegundoApellido, 
                                       c.CLI_RAZON_SOCIAL as RazonSocial, c.CLI_FECHA_NACIMIENTO as FechaNacimiento, c.CLI_GENERO as Genero, c.CLI_ESTADO as Estado,
                                       tc.TCL_NOMBRE as TipoClienteNombre, td.TDO_NOMBRE as TipoDocumentoNombre
                                FROM ALP_CLIENTE c
                                JOIN ALP_TIPO_CLIENTE tc ON tc.TCL_TIPO_CLIENTE = c.TCL_TIPO_CLIENTE
                                JOIN ALP_TIPO_DOCUMENTO td ON td.TDO_TIPO_DOCUMENTO = c.TDO_TIPO_DOCUMENTO
                                WHERE c.CLI_CLIENTE = :clienteId";
            var cliente = await connection.QueryFirstOrDefaultAsync<ClienteDetalleDto>(qMaestro, new { clienteId });
            
            if (cliente == null) return null;

            // 2. Emails
            string qEmails = "SELECT CLE_CLIENTE_EMAIL as Id, CLI_CLIENTE as ClienteId, CLE_EMAIL as Email, CLE_PRINCIPAL as EsPrincipal, CLE_ESTADO as Estado FROM ALP_CLIENTE_EMAIL WHERE CLI_CLIENTE = :clienteId";
            cliente.Emails = (await connection.QueryAsync<ClienteEmail>(qEmails, new { clienteId })).ToList();

            // 3. Telefonos
            string qTelefonos = "SELECT CLT_CLIENTE_TELEFONO as Id, CLI_CLIENTE as ClienteId, CLT_TIPO as Tipo, CLT_NUMERO as Numero, CLT_PRINCIPAL as EsPrincipal, CLT_ESTADO as Estado FROM ALP_CLIENTE_TELEFONO WHERE CLI_CLIENTE = :clienteId";
            cliente.Telefonos = (await connection.QueryAsync<ClienteTelefono>(qTelefonos, new { clienteId })).ToList();

            // 4. Direcciones
            string qDirecciones = "SELECT CLD_CLIENTE_DIRECCION as Id, CLI_CLIENTE as ClienteId, CIU_CIUDAD as CiudadId, CLD_TIPO as Tipo, CLD_DIRECCION_LINEA1 as DireccionLinea1, CLD_DIRECCION_LINEA2 as DireccionLinea2, CLD_CODIGO_POSTAL as CodigoPostal, CLD_REFERENCIA as Referencia, CLD_PRINCIPAL as EsPrincipal, CLD_ESTADO as Estado FROM ALP_CLIENTE_DIRECCION WHERE CLI_CLIENTE = :clienteId";
            cliente.Direcciones = (await connection.QueryAsync<ClienteDireccion>(qDirecciones, new { clienteId })).ToList();

            // 5. Preferencias
            string qPrefs = "SELECT CLI_CLIENTE as ClienteId, IDI_IDIOMA as IdiomaId, MON_MONEDA as MonedaId, CLP_ACEPTA_MARKETING as AceptaMarketing, CLP_ACEPTA_SMS as AceptaSms, CLP_ACEPTA_EMAIL as AceptaEmail FROM ALP_CLIENTE_PREFERENCIA WHERE CLI_CLIENTE = :clienteId";
            cliente.Preferencias = await connection.QueryFirstOrDefaultAsync<ClientePreferencia>(qPrefs, new { clienteId });

            return cliente;
        }
        #endregion

        #region Detalles
        public async Task<int> AgregarEmailAsync(ClienteEmail email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", email.ClienteId);
            parameters.Add("p_cle_email", email.Email);
            parameters.Add("p_cle_principal", email.EsPrincipal);
            parameters.Add("p_cle_estado", email.Estado);
            parameters.Add("p_cle_cliente_email_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_AGREGAR_EMAIL_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_cle_cliente_email_out");
        }

        public async Task ActualizarEmailAsync(ClienteEmail email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cle_cliente_email", email.Id);
            parameters.Add("p_cle_email", email.Email);
            parameters.Add("p_cle_principal", email.EsPrincipal);
            parameters.Add("p_cle_estado", email.Estado);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_ACTUALIZAR_EMAIL_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task MarcarEmailPrincipalAsync(int emailId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_MARCAR_EMAIL_PRINCIPAL", new { p_cle_cliente_email = emailId }, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarEmailAsync(int emailId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_ELIMINAR_EMAIL_CLIENTE", new { p_cle_cliente_email = emailId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AgregarTelefonoAsync(ClienteTelefono telefono)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", telefono.ClienteId);
            parameters.Add("p_clt_tipo", telefono.Tipo);
            parameters.Add("p_clt_numero", telefono.Numero);
            parameters.Add("p_clt_principal", telefono.EsPrincipal);
            parameters.Add("p_clt_estado", telefono.Estado);
            parameters.Add("p_clt_cliente_telefono_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_AGREGAR_TELEFONO_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_clt_cliente_telefono_out");
        }

        public async Task ActualizarTelefonoAsync(ClienteTelefono telefono)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_clt_cliente_telefono", telefono.Id);
            parameters.Add("p_clt_tipo", telefono.Tipo);
            parameters.Add("p_clt_numero", telefono.Numero);
            parameters.Add("p_clt_principal", telefono.EsPrincipal);
            parameters.Add("p_clt_estado", telefono.Estado);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_ACTUALIZAR_TELEFONO_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task MarcarTelefonoPrincipalAsync(int telefonoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_MARCAR_TELEFONO_PRINCIPAL", new { p_clt_cliente_telefono = telefonoId }, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarTelefonoAsync(int telefonoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_ELIMINAR_TELEFONO_CLIENTE", new { p_clt_cliente_telefono = telefonoId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AgregarDireccionAsync(ClienteDireccion direccion)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", direccion.ClienteId);
            parameters.Add("p_pai_pais", direccion.PaisId);
            parameters.Add("p_dep_departamento", direccion.DepartamentoId);
            parameters.Add("p_ciu_ciudad", direccion.CiudadId);
            parameters.Add("p_cld_tipo", direccion.Tipo);
            parameters.Add("p_cld_direccion_linea1", direccion.DireccionLinea1);
            parameters.Add("p_cld_direccion_linea2", direccion.DireccionLinea2);
            parameters.Add("p_cld_codigo_postal", direccion.CodigoPostal);
            parameters.Add("p_cld_referencia", direccion.Referencia);
            parameters.Add("p_cld_principal", direccion.EsPrincipal);
            parameters.Add("p_cld_cliente_direccion_out", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_AGREGAR_DIRECCION_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("p_cld_cliente_direccion_out");
        }

        public async Task ActualizarDireccionAsync(ClienteDireccion direccion)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cld_cliente_direccion", direccion.Id);
            parameters.Add("p_pai_pais", direccion.PaisId);
            parameters.Add("p_dep_departamento", direccion.DepartamentoId);
            parameters.Add("p_ciu_ciudad", direccion.CiudadId);
            parameters.Add("p_cld_tipo", direccion.Tipo);
            parameters.Add("p_cld_direccion_linea1", direccion.DireccionLinea1);
            parameters.Add("p_cld_direccion_linea2", direccion.DireccionLinea2);
            parameters.Add("p_cld_codigo_postal", direccion.CodigoPostal);
            parameters.Add("p_cld_referencia", direccion.Referencia);
            parameters.Add("p_cld_principal", direccion.EsPrincipal);
            parameters.Add("p_cld_estado", direccion.Estado);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_ACTUALIZAR_DIRECCION_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task MarcarDireccionPrincipalAsync(int direccionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_MARCAR_DIRECCION_PREDETERMINADA", new { p_cld_cliente_direccion = direccionId }, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarDireccionAsync(int direccionId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("PKG_CLIENTES.SP_ELIMINAR_DIRECCION_CLIENTE", new { p_cld_cliente_direccion = direccionId }, commandType: CommandType.StoredProcedure);
        }

        public async Task GuardarPreferenciasAsync(ClientePreferencia pref)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_cli_cliente", pref.ClienteId);
            parameters.Add("p_idi_idioma", pref.IdiomaId);
            parameters.Add("p_mon_moneda", pref.MonedaId);
            parameters.Add("p_clp_acepta_marketing", pref.AceptaMarketing);
            parameters.Add("p_clp_acepta_sms", pref.AceptaSms);
            parameters.Add("p_clp_acepta_email", pref.AceptaEmail);

            await connection.ExecuteAsync("PKG_CLIENTES.SP_GUARDAR_PREFERENCIAS_CLIENTE", parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}
