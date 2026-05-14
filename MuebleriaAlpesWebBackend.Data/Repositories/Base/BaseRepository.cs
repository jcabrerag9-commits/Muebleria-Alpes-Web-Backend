using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using MuebleriaAlpesWebBackend.Domain.Models;

namespace MuebleriaAlpesWebBackend.Data.Repositories.Base
{
    public abstract class BaseRepository<T>
    {
        protected readonly ILogger<T> _logger;

        protected BaseRepository(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected void ValidarParametroNulo(object? parametro, string nombreParametro)
        {
            if (parametro == null)
            {
                _logger.LogWarning("Parámetro nulo detectado: {Parametro}", nombreParametro);
                throw new ArgumentNullException(nombreParametro, $"El parámetro {nombreParametro} no puede ser nulo.");
            }
        }

        protected string ExtraerMensajeOracle(OracleException ex)
        {
            string raw = ex.Message ?? string.Empty;
            string[] lines = raw.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                
                int colon = trimmed.IndexOf(':');
                if (colon >= 0 && colon < trimmed.Length - 1)
                {
                    var afterColon = trimmed[(colon + 1)..].Trim();
                    if (!string.IsNullOrEmpty(afterColon) && !afterColon.StartsWith("ORA-"))
                        return afterColon;
                }
            }
            return raw.Length > 300 ? raw[..300] : raw;
        }

        protected InventarioResponse<R> MapearErrorOracle<R>(OracleException ex, R? dataDefault = default)
        {
            string mensaje = ExtraerMensajeOracle(ex);
            string status = (ex.Number >= 20000 && ex.Number <= 20999) ? "RECHAZADO" : "ERROR";
            
            _logger.LogError(ex, "[ORA-{ErrorNumber}] Error en base de datos: {Mensaje}", ex.Number, mensaje);
            
            return new InventarioResponse<R> 
            { 
                Success = false,
                Resultado = status, // Estado interno para controladores (EXITO/RECHAZADO/ERROR)
                Mensaje = mensaje, 
                Data = dataDefault,
                Errores = new List<string> { $"ORA-{ex.Number}: {mensaje}" }
            };
        }

        protected void LogOperacionInicio(string operacion, object? payload = null)
        {
            _logger.LogInformation("Iniciando operación: {Operacion}. Payload: {@Payload}", operacion, payload);
        }

        protected void LogOperacionExito(string operacion, object? resultado = null)
        {
            _logger.LogInformation("Operación finalizada con éxito: {Operacion}. Resultado: {@Resultado}", operacion, resultado);
        }
    }
}
