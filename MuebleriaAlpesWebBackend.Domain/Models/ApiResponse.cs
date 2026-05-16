using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    /// <summary>
    /// Wrapper universal para respuestas de la API ERP (H.5 Normalized Contract)
    /// </summary>
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        [JsonPropertyName("resultado")]
        public string Resultado { get; set; } = "OK";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "OK";

        [JsonPropertyName("mensaje")]
        public string Mensaje { get => Message; set => Message = value; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errores")]
        public List<string> Errores { get; set; } = new List<string>();

        // --- ALIAS DE COMPATIBILIDAD ---
        
        [JsonIgnore]
        public bool IsSuccess => Success;
    }
}
