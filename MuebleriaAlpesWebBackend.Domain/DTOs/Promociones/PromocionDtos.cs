using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Promociones
{
    // ── Constantes de dominio ────────────────────────────────────────────────
    public static class TipoPromocion
    {
        public const string Porcentaje   = "PORCENTAJE";
        public const string MontoFijo    = "MONTO_FIJO";
        public const string DosXUno      = "2X1";
        public const string EnvioGratis  = "ENVIO_GRATIS";
        public const string CompraMinima = "COMPRA_MINIMA";
        public static readonly string[] Todos = [Porcentaje, MontoFijo, DosXUno, EnvioGratis, CompraMinima];
    }

    public static class EstadoPromocion
    {
        public const string Activo   = "ACTIVO";
        public const string Inactivo = "INACTIVO";
        public const string Expirado = "EXPIRADO";
    }

    // ── DTO Crear ────────────────────────────────────────────────────────────
    public class PromocionCreateDto
    {
        [Required(ErrorMessage = "El código es requerido.")]
        [MaxLength(50)]
        public string PrmCodigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [MaxLength(150)]
        public string PrmNombre { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? PrmDescripcion { get; set; }

        [Required(ErrorMessage = "El tipo es requerido.")]
        public string PrmTipo { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a 0.")]
        public decimal? PrmValor { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateTime PrmFechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida.")]
        public DateTime PrmFechaFin { get; set; }

        public List<PromocionProductoCreateDto>? Productos { get; set; }
    }

    // ── DTO Actualizar ───────────────────────────────────────────────────────
    public class PromocionUpdateDto
    {
        [MaxLength(150)]
        public string? PrmNombre { get; set; }

        [MaxLength(1000)]
        public string? PrmDescripcion { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? PrmValor { get; set; }

        public DateTime? PrmFechaInicio { get; set; }
        public DateTime? PrmFechaFin { get; set; }
        public string? PrmEstado { get; set; }
    }

    // ── DTO Respuesta completa ───────────────────────────────────────────────
    public class PromocionResponseDto
    {
        public long PrmPromocion { get; set; }
        public string PrmCodigo { get; set; } = string.Empty;
        public string PrmNombre { get; set; } = string.Empty;
        public string? PrmDescripcion { get; set; }
        public string PrmTipo { get; set; } = string.Empty;
        public decimal? PrmValor { get; set; }
        public DateTime PrmFechaInicio { get; set; }
        public DateTime PrmFechaFin { get; set; }
        public string PrmEstado { get; set; } = string.Empty;
        public bool EstaVigente =>
            PrmEstado == EstadoPromocion.Activo &&
            PrmFechaInicio <= DateTime.Now &&
            PrmFechaFin >= DateTime.Now;
        public List<PromocionProductoResponseDto> Productos { get; set; } = [];
    }

    // ── DTO Listado resumido ─────────────────────────────────────────────────
    public class PromocionListDto
    {
        public long PrmPromocion { get; set; }
        public string PrmCodigo { get; set; } = string.Empty;
        public string PrmNombre { get; set; } = string.Empty;
        public string PrmTipo { get; set; } = string.Empty;
        public decimal? PrmValor { get; set; }
        public DateTime PrmFechaInicio { get; set; }
        public DateTime PrmFechaFin { get; set; }
        public string PrmEstado { get; set; } = string.Empty;
    }

    // ── DTOs PromocionProducto ───────────────────────────────────────────────
    public class PromocionProductoCreateDto
    {
        public long? ProProducto { get; set; }
        public long? PvaProductoVariante { get; set; }
    }

    public class PromocionProductoResponseDto
    {
        public long PpoPromocionProducto { get; set; }
        public long? ProProducto { get; set; }
        public long? PvaProductoVariante { get; set; }
        public string PpoEstado { get; set; } = string.Empty;
        public DateTime PpoFechaCreacion { get; set; }
    }
}
