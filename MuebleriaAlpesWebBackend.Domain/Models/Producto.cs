using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class Producto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de mueble es obligatorio")]
        [JsonPropertyName("tipoMueble")]
        public int TipoMueble { get; set; }

        [StringLength(50, ErrorMessage = "El SKU no puede exceder los 50 caracteres")]
        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres")]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción corta no puede exceder los 500 caracteres")]
        [JsonPropertyName("descripcionCorta")]
        public string? DescripcionCorta { get; set; }

        [JsonPropertyName("descripcionLarga")]
        public string? DescripcionLarga { get; set; }

        [Range(0, 10000, ErrorMessage = "El peso debe ser un valor positivo")]
        [JsonPropertyName("peso")]
        public decimal? Peso { get; set; }

        [RegularExpression("^[SN]$")]
        [JsonPropertyName("esConfigurable")]
        public string EsConfigurable { get; set; } = "N";

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = "BORRADOR";

        [JsonPropertyName("publicado")]
        public string Publicado { get; set; } = "N";

        [JsonPropertyName("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        // Propiedades de Enriquecimiento (No mapeadas en la tabla ALP_PRODUCTO)
        [JsonPropertyName("precioVigente")]
        public decimal? PrecioVigente { get; set; }

        [JsonPropertyName("precioOferta")]
        public decimal? PrecioOferta { get; set; }
    }

    public class DimensionProducto
    {
        [Required]
        public int ProductoId { get; set; }

        [Range(0.01, 1000)]
        public decimal Alto { get; set; }

        [Range(0.01, 1000)]
        public decimal Ancho { get; set; }

        [Range(0.01, 1000)]
        public decimal Largo { get; set; }

        [Required]
        public int UnidadId { get; set; }
    }

    public class ResenaProducto
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Calificacion { get; set; }

        [StringLength(100)]
        public string Titulo { get; set; }

        public string Comentario { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
        public string Aprobada { get; set; } = "N";
    }
}
