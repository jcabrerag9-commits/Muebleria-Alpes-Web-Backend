using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace MuebleriaAlpesWebBackend.Data.Connection
{
    public enum OracleMappingType
    {
        Varchar2 = OracleDbType.Varchar2,
        Int32 = OracleDbType.Int32,
        Decimal = OracleDbType.Decimal,
        RefCursor = OracleDbType.RefCursor,
        Date = OracleDbType.Date,
        Double = OracleDbType.Double,
        Long = OracleDbType.Long,
        Byte = OracleDbType.Byte,
        Char = OracleDbType.Char,
        XmlType = OracleDbType.XmlType,
        Blob = OracleDbType.Blob,
        Clob = OracleDbType.Clob
    }

    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters _dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> _oracleParameters = new List<OracleParameter>();

        public void Add(string name, object? value = null, OracleMappingType? dbType = null, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            if (dbType.HasValue)
            {
                var oracleParameter = new OracleParameter
                {
                    ParameterName = name,
                    OracleDbType = (OracleDbType)dbType.Value,
                    Direction = direction,
                    Value = value
                };

                if (size.HasValue)
                {
                    oracleParameter.Size = size.Value;
                }

                _oracleParameters.Add(oracleParameter);
                
                // Hardening: También lo agregamos a Dapper para que sepa que el parámetro existe
                // aunque el valor real se maneje vía OracleParameter directo en AddParameters
                _dynamicParameters.Add(name, value, direction: direction, size: size);
            }
            else
            {
                _dynamicParameters.Add(name, value, direction: direction, size: size);
            }
        }

        public void Add(string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            _dynamicParameters.Add(name, value, dbType, direction, size);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)_dynamicParameters).AddParameters(command, identity);

            if (command is OracleCommand oracleCommand)
            {
                oracleCommand.BindByName = true;
                foreach (var p in _oracleParameters)
                {
                    if (oracleCommand.Parameters.Contains(p.ParameterName))
                    {
                        oracleCommand.Parameters.Remove(oracleCommand.Parameters[p.ParameterName]);
                    }
                    oracleCommand.Parameters.Add(p);
                }
            }
        }

        public T Get<T>(string name)
        {
            var oracleParam = _oracleParameters.Find(p => p.ParameterName == name);
            if (oracleParam != null)
            {
                var val = oracleParam.Value;
                if (val == DBNull.Value || val == null) return default!;

                // Manejo de OracleDecimal u otros tipos específicos de Oracle (H.8.7)
                try 
                {
                    // Si T es nullable, obtenemos el tipo base
                    Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                    
                    // Si el valor ya es T, casting directo
                    if (val is T variable) return variable;

                    // Si es OracleDecimal, usamos su representación en string o su propiedad Value
                    if (val.GetType().Name == "OracleDecimal")
                    {
                        dynamic od = val;
                        if (od.IsNull) return default!;
                        
                        // Conversión segura vía string para evitar problemas de casting directo
                        string stringVal = od.ToString();
                        return (T)System.Convert.ChangeType(stringVal, targetType);
                    }

                    // Conversión genérica usando string como puente seguro
                    return (T)System.Convert.ChangeType(val.ToString()!, targetType);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[OracleDynamicParameters ERROR] Fallo al convertir {name} ({val?.GetType().Name}) a {typeof(T).Name}: {ex.Message}");
                    try {
                        return (T)System.Convert.ChangeType(val, typeof(T));
                    } catch {
                        return (T)val; 
                    }
                }
            }

            return _dynamicParameters.Get<T>(name);
        }
    }
}
