using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class ProductoImagen
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }

        [RegularExpression("^(PRINCIPAL|GALERIA)$")]
        public string Tipo { get; set; } = "GALERIA";

        [Range(1, 100)]
        public int Orden { get; set; } = 1;

        public string Estado { get; set; } = "ACTIVO";
    }

    public class ProductoTraduccion
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int IdiomaId { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string DescripcionCorta { get; set; }

        public string DescripcionLarga { get; set; }
    }
}
