using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MuebleriaAlpesWebBackend.Data.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public TestRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> ProbarConexionAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var resultado = await connection.QueryFirstAsync<string>(
                "SELECT 'OK' FROM DUAL"
            );

            return resultado;
        }
    }
}
