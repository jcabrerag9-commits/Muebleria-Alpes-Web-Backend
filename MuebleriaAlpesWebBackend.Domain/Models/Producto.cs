using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de mueble es obligatorio")]
        public int TipoMueble { get; set; }

        [Required(ErrorMessage = "El SKU es obligatorio")]
        [StringLength(50, ErrorMessage = "El SKU no puede exceder los 50 caracteres")]
        public string Sku { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción corta no puede exceder los 500 caracteres")]
        public string DescripcionCorta { get; set; }

        public string DescripcionLarga { get; set; }

        [Range(0, 10000, ErrorMessage = "El peso debe ser un valor positivo")]
        public decimal? Peso { get; set; }

        [RegularExpression("^[SN]$")]
        public string EsConfigurable { get; set; } = "N";

        public string Estado { get; set; } = "BORRADOR";
        public string Publicado { get; set; } = "N";
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

        [StringLength(10)]
        public string Unidad { get; set; } = "cm";
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
