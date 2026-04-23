using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            // Si es una excepción de Oracle con códigos personalizados (-20000 al -20999)
            // los mostramos como BadRequest con el mensaje original.
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Ocurrió un error inesperado en el servidor.";

            if (exception.Message.Contains("ORA-20"))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = ExtractOracleMessage(exception.Message);
            }
            else if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { 
                error = message,
                status = (int)statusCode
            });

            return context.Response.WriteAsync(result);
        }

        private static string ExtractOracleMessage(string fullMessage)
        {
            // Extraer el mensaje limpio de ORA-20XXX: mensaje
            try 
            {
                int start = fullMessage.IndexOf("ORA-20");
                if (start == -1) return fullMessage;
                
                string sub = fullMessage.Substring(start);
                int end = sub.IndexOf("\n");
                if (end == -1) end = sub.Length;
                
                string line = sub.Substring(0, end);
                int msgStart = line.IndexOf(":") + 1;
                return line.Substring(msgStart).Trim();
            }
            catch 
            {
                return fullMessage;
            }
        }
    }
}
