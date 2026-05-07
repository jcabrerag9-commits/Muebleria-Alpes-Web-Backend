using System;
using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class PrecioProducto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }

        public int? VarianteId { get; set; }

        [Required(ErrorMessage = "La moneda es obligatoria")]
        public int MonedaId { get; set; }

        [Required]
        [RegularExpression("^(REGULAR|OFERTA|MAYORISTA)$", ErrorMessage = "Tipo de precio inválido")]
        public string Tipo { get; set; } = "REGULAR";

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, 1000000)]
        public decimal? PrecioOferta { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Now;

        public DateTime? FechaFin { get; set; }

        public string Estado { get; set; } = "ACTIVO";
    }

    public class HistorialPrecio
    {
        public int Id { get; set; }
        public int PrecioId { get; set; }
        public decimal PrecioAnterior { get; set; }
        public decimal PrecioNuevo { get; set; }
        public int? UsuarioId { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class ActualizarPrecioRequest
    {
        [Required]
        public int PrecioId { get; set; }
        public decimal? NuevoPrecio { get; set; }
        public decimal? NuevoPrecioOferta { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? UsuarioId { get; set; }
        public string Motivo { get; set; }
    }
}
