using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Data.Connection
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly List<OracleParameter> _parameters = new();

        public void Add(
            string name,
            object? value = null,
            OracleDbType? dbType = null,
            ParameterDirection direction = ParameterDirection.Input,
            int? size = null)
        {
            var parameter = new OracleParameter
            {
                ParameterName = name,
                Value = value ?? DBNull.Value,
                Direction = direction
            };

            if (dbType.HasValue)
                parameter.OracleDbType = dbType.Value;

            if (size.HasValue)
                parameter.Size = size.Value;

            _parameters.Add(parameter);
        }

        public T? Get<T>(string name)
        {
            var parameter = _parameters.FirstOrDefault(p => p.ParameterName == name);

            if (parameter == null || parameter.Value == null || parameter.Value == DBNull.Value)
                return default;

            var value = parameter.Value;

            if (value is Oracle.ManagedDataAccess.Types.OracleDecimal oracleDecimal)
            {
                if (oracleDecimal.IsNull)
                    return default;

                value = oracleDecimal.Value;
            }

            if (value is Oracle.ManagedDataAccess.Types.OracleString oracleString)
            {
                if (oracleString.IsNull)
                    return default;

                value = oracleString.Value;
            }

            var stringValue = value.ToString();

            if (string.IsNullOrWhiteSpace(stringValue) || stringValue.Equals("null", StringComparison.OrdinalIgnoreCase))
                return default;

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return (T)Convert.ChangeType(stringValue, targetType);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var oracleCommand = (OracleCommand)command;

            foreach (var parameter in _parameters)
            {
                oracleCommand.Parameters.Add(parameter);
            }
        }
    }
}
