using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Entities.RecursosHumanos;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;

namespace MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos
{
    public class NominaRepository : INominaRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public NominaRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseSpDTO> CrearAsync(CrearNominaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_periodo", dto.Periodo, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_fecha_inicio", dto.FechaInicio, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_fecha_fin", dto.FechaFin, OracleDbType.Date, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_CREAR_NOMINA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters, "p_id");
        }

        public async Task<ResponseSpDTO> CambiarEstadoAsync(int nominaId, CambiarEstadoNominaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_nomina_id", nominaId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_estado", dto.Estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_CAMBIAR_ESTADO_NOMINA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters);
        }

        public async Task<NominaResponseDTO?> ObtenerPorIdAsync(int nominaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_nomina_id", nominaId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entity = await connection.QueryFirstOrDefaultAsync<Nomina>(
                "PKG_RH_NOMINA.SP_OBTENER_NOMINA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (entity == null)
                return null;

            return MapNomina(entity);
        }

        public async Task<IEnumerable<NominaResponseDTO>> ListarAsync(string? estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_estado", estado, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<Nomina>(
                "PKG_RH_NOMINA.SP_LISTAR_NOMINAS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(MapNomina);
        }

        public async Task<ResponseSpDTO> AgregarEmpleadoAsync(int nominaId, AgregarEmpleadoNominaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_nomina_id", nominaId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_empleado_id", dto.EmpleadoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_salario_base", dto.SalarioBase, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_detalle_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_AGREGAR_EMPLEADO_NOMINA",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters, "p_detalle_id");
        }

        public async Task<ResponseSpDTO> CalcularDetalleAsync(int detalleId, CalcularNominaDetalleDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_detalle_id", detalleId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_CALCULAR_NOMINA_DETALLE",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters);
        }

        public async Task<IEnumerable<NominaDetalleResponseDTO>> ListarDetalleAsync(int nominaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_nomina_id", nominaId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<NominaDetalle>(
                "PKG_RH_NOMINA.SP_LISTAR_NOMINA_DETALLE",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new NominaDetalleResponseDTO
            {
                Id = x.NOD_NOMINA_DETALLE,
                NominaId = x.RHN_NOMINA,
                EmpleadoId = x.EMP_EMPLEADO,
                Empleado = x.EMPLEADO,
                SalarioBase = x.NOD_SALARIO_BASE,
                TotalIngresos = x.NOD_TOTAL_INGRESOS,
                TotalDeducciones = x.NOD_TOTAL_DEDUCCIONES,
                SalarioNeto = x.NOD_SALARIO_NETO,
                Estado = x.NOD_ESTADO
            });
        }

        public async Task<ResponseSpDTO> AgregarIngresoAsync(int detalleId, AgregarIngresoNominaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_detalle_id", detalleId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_tipo_pago_id", dto.TipoPagoId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_monto", dto.Monto, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_AGREGAR_INGRESO",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters, "p_id");
        }

        public async Task<IEnumerable<NominaIngresoResponseDTO>> ListarIngresosAsync(int detalleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_detalle_id", detalleId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<NominaIngreso>(
                "PKG_RH_NOMINA.SP_LISTAR_INGRESOS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new NominaIngresoResponseDTO
            {
                Id = x.NNI_NOMINA_INGRESO,
                DetalleId = x.NOD_NOMINA_DETALLE,
                TipoPagoId = x.TIP_TIPO_PAGO,
                TipoPago = x.TIPO_PAGO,
                Monto = x.NNI_MONTO
            });
        }

        public async Task<ResponseSpDTO> AgregarDeduccionAsync(int detalleId, AgregarDeduccionNominaDTO dto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_detalle_id", detalleId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_tipo_deduccion_id", dto.TipoDeduccionId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_monto", dto.Monto, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("p_usuario_id", dto.UsuarioId, OracleDbType.Int32, ParameterDirection.Input);

            parameters.Add("p_resultado", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 50);
            parameters.Add("p_mensaje", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Output, size: 500);
            parameters.Add("p_id", dbType: OracleDbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "PKG_RH_NOMINA.SP_AGREGAR_DEDUCCION",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return MapSpResponse(parameters, "p_id");
        }

        public async Task<IEnumerable<NominaDeduccionResponseDTO>> ListarDeduccionesAsync(int detalleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_detalle_id", detalleId, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("p_cursor", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var entities = await connection.QueryAsync<NominaDeduccion>(
                "PKG_RH_NOMINA.SP_LISTAR_DEDUCCIONES",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return entities.Select(x => new NominaDeduccionResponseDTO
            {
                Id = x.NDE_NOMINA_DEDUCCION,
                DetalleId = x.NOD_NOMINA_DETALLE,
                TipoDeduccionId = x.TDE_TIPO_DEDUCCION,
                TipoDeduccion = x.TIPO_DEDUCCION,
                Monto = x.NDE_MONTO
            });
        }

        private static NominaResponseDTO MapNomina(Nomina x)
        {
            return new NominaResponseDTO
            {
                Id = x.RHN_NOMINA,
                Periodo = x.RHN_PERIODO,
                FechaInicio = x.RHN_FECHA_INICIO,
                FechaFin = x.RHN_FECHA_FIN,
                Estado = x.RHN_ESTADO
            };
        }

        private static ResponseSpDTO MapSpResponse(OracleDynamicParameters parameters, string? idParameter = null)
        {
            return new ResponseSpDTO
            {
                Resultado = parameters.Get<string>("p_resultado") ?? string.Empty,
                Mensaje = parameters.Get<string>("p_mensaje") ?? string.Empty,
                Id = idParameter == null ? null : parameters.Get<int?>(idParameter)
            };
        }
    }
}
