using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class ProductoVariante
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Sku { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string CodigoBarras { get; set; }

        [Url]
        public string ImagenUrl { get; set; }

        [RegularExpression("^(ACTIVO|INACTIVO|DESCONTINUADO)$")]
        public string Estado { get; set; } = "ACTIVO";
    }
}
