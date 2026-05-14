using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.DTOs.Admin;
using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public AdminRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BaseResponse<AdminListarOrdenesDataDto>> ListarOrdenesAsync(ListarOrdenesFiltroDto filtro)
        {
            var ordenes = new List<AdminOrdenDto>();

            try
            {
                using var connection = (OracleConnection)_connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SP_ADMIN_LISTAR_ORDENES";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_fecha_inicio", OracleDbType.Date, filtro.FechaInicio.HasValue ? filtro.FechaInicio.Value : (object)DBNull.Value, ParameterDirection.Input);
                cmd.Parameters.Add("p_fecha_fin", OracleDbType.Date, filtro.FechaFin.HasValue ? filtro.FechaFin.Value : (object)DBNull.Value, ParameterDirection.Input);
                cmd.Parameters.Add("p_estado_id", OracleDbType.Int32, filtro.EstadoId.HasValue ? filtro.EstadoId.Value : (object)DBNull.Value, ParameterDirection.Input);

                var pCursor = new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(pCursor);

                await cmd.ExecuteNonQueryAsync();

                if (pCursor.Value is OracleRefCursor refCursor && !refCursor.IsNull)
                {
                    using var reader = refCursor.GetDataReader();
                    while (await reader.ReadAsync())
                    {
                        ordenes.Add(new AdminOrdenDto
                        {
                            OrdenId = reader.GetInt32(reader.GetOrdinal("VEN_ORDEN_VENTA")),
                            NumeroOrden = reader.GetString(reader.GetOrdinal("VEN_NUMERO_ORDEN")),
                            FechaOrden = reader.GetDateTime(reader.GetOrdinal("VEN_FECHA_ORDEN")),
                            Cliente = reader.GetString(reader.GetOrdinal("CLIENTE")),
                            Total = reader.GetDecimal(reader.GetOrdinal("VEN_TOTAL")),
                            Estado = reader.GetString(reader.GetOrdinal("ESTADO"))
                        });
                    }
                }

                return new BaseResponse<AdminListarOrdenesDataDto>
                {
                    Resultado = "EXITO",
                    Mensaje = "Órdenes recuperadas correctamente",
                    Data = new AdminListarOrdenesDataDto { Ordenes = ordenes }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdminListarOrdenesDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Error al listar las órdenes: " + ex.Message
                };
            }
        }

        public async Task<BaseResponse<AdminListarPagosDataDto>> ListarPagosAsync()
        {
            var pagos = new List<AdminPagoDto>();

            try
            {
                using var connection = (OracleConnection)_connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SP_ADMIN_LISTAR_PAGOS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                var pCursor = new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(pCursor);

                await cmd.ExecuteNonQueryAsync();

                if (pCursor.Value is OracleRefCursor refCursor && !refCursor.IsNull)
                {
                    using var reader = refCursor.GetDataReader();
                    while (await reader.ReadAsync())
                    {
                        pagos.Add(new AdminPagoDto
                        {
                            PagoId = reader.GetInt32(reader.GetOrdinal("PAG_PAGO")),
                            NumeroOrden = reader.GetString(reader.GetOrdinal("VEN_NUMERO_ORDEN")),
                            Metodo = reader.GetString(reader.GetOrdinal("METODO")),
                            Monto = reader.GetDecimal(reader.GetOrdinal("PAG_MONTO")),
                            Estado = reader.GetString(reader.GetOrdinal("ESTADO")),
                            FechaPago = reader.GetDateTime(reader.GetOrdinal("PAG_FECHA_PAGO"))
                        });
                    }
                }

                return new BaseResponse<AdminListarPagosDataDto>
                {
                    Resultado = "EXITO",
                    Mensaje = "Pagos recuperados correctamente",
                    Data = new AdminListarPagosDataDto { Pagos = pagos }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdminListarPagosDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Error al listar los pagos: " + ex.Message
                };
            }
        }

        public async Task<BaseResponse<AdminListarFacturasDataDto>> ListarFacturasAsync()
        {
            var facturas = new List<AdminFacturaDto>();

            try
            {
                using var connection = (OracleConnection)_connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SP_ADMIN_LISTAR_FACTURAS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                var pCursor = new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add(pCursor);

                await cmd.ExecuteNonQueryAsync();

                if (pCursor.Value is OracleRefCursor refCursor && !refCursor.IsNull)
                {
                    using var reader = refCursor.GetDataReader();
                    while (await reader.ReadAsync())
                    {
                        facturas.Add(new AdminFacturaDto
                        {
                            FacturaId = reader.GetInt32(reader.GetOrdinal("FAC_FACTURA")),
                            Numero = reader.IsDBNull(reader.GetOrdinal("FAC_NUMERO")) ? string.Empty : reader.GetString(reader.GetOrdinal("FAC_NUMERO")),
                            Serie = reader.IsDBNull(reader.GetOrdinal("FAC_SERIE")) ? string.Empty : reader.GetString(reader.GetOrdinal("FAC_SERIE")),
                            NumeroOrden = reader.GetString(reader.GetOrdinal("VEN_NUMERO_ORDEN")),
                            Cliente = reader.GetString(reader.GetOrdinal("CLIENTE")),
                            Total = reader.GetDecimal(reader.GetOrdinal("FAC_TOTAL")),
                            Estado = reader.GetString(reader.GetOrdinal("FAC_ESTADO")),
                            FechaEmision = reader.GetDateTime(reader.GetOrdinal("FAC_FECHA_EMISION"))
                        });
                    }
                }

                return new BaseResponse<AdminListarFacturasDataDto>
                {
                    Resultado = "EXITO",
                    Mensaje = "Facturas recuperadas correctamente",
                    Data = new AdminListarFacturasDataDto { Facturas = facturas }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AdminListarFacturasDataDto>
                {
                    Resultado = "ERROR",
                    Mensaje = "Error al listar las facturas: " + ex.Message
                };
            }
        }
    }
}
