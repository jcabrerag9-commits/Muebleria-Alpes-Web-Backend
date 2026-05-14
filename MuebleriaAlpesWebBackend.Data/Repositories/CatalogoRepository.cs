using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Models;
using MuebleriaAlpesWebBackend.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class CatalogoRepository : BaseRepository<CatalogoRepository>, ICatalogoRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public CatalogoRepository(OracleConnectionFactory connectionFactory, ILogger<CatalogoRepository> logger) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<CatalogoItemDTO>> ListarTiposMuebleAsync()
        {
            LogOperacionInicio(nameof(ListarTiposMuebleAsync));
            var resultados = new List<CatalogoItemDTO>();
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                if (connection.State != ConnectionState.Open) connection.Open();

                using var command = (OracleCommand)connection.CreateCommand();
                command.CommandText = "PKG_CATALOGOS_PRODUCTOS.SP_LISTAR_TIPOS_MUEBLE";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    resultados.Add(new CatalogoItemDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("TMU_TIPO_MUEBLE")),
                        Nombre = reader.GetString(reader.GetOrdinal("TMU_NOMBRE")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("TMU_DESCRIPCION")) ? null : reader.GetString(reader.GetOrdinal("TMU_DESCRIPCION"))
                    });
                }
                
                LogOperacionExito(nameof(ListarTiposMuebleAsync), resultados.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tipos de mueble.");
            }
            return resultados;
        }
    }
}

