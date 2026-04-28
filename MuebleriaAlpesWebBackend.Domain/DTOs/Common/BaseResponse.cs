namespace MuebleriaAlpesWebBackend.Domain.DTOs.Common
{
    /// <summary>
    /// Respuesta base para SPs que solo retornan p_resultado y p_mensaje.
    /// </summary>
    public class BaseResponse
    {
        public string Resultado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Exitoso => Resultado == "EXITO";
    }

    /// <summary>
    /// Respuesta genérica para SPs que retornan datos adicionales en parámetros OUT.
    /// </summary>
    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}
