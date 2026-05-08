namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class ApiResponse<T>
    {
        public string Resultado { get; set; } = "EXITO";
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSuccess => Resultado == "EXITO";
    }
}
