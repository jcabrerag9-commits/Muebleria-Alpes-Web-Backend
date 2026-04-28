using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Devoluciones
{
    // ── Constantes de dominio ────────────────────────────────────────────────
    public static class EstadoDevolucion
    {
        public const string Solicitada  = "SOLICITADA";
        public const string EnRevision  = "EN_REVISION";
        public const string Aprobada    = "APROBADA";
        public const string Rechazada   = "RECHAZADA";
        public const string Completada  = "COMPLETADA";
    }

    public static class EstadoDetalleDevolucion
    {
        public const string Pendiente     = "PENDIENTE";
        public const string Recibido      = "RECIBIDO";
        public const string Inspeccionado = "INSPECCIONADO";
        public const string Reintegrado   = "REINTEGRADO";
    }

    // ── DTO Crear devolución ─────────────────────────────────────────────────
    public class DevolucionCreateDto
    {
        [Required(ErrorMessage = "La orden de venta es requerida.")]
        public long VenOrdenVenta { get; set; }

        [Required(ErrorMessage = "El cliente es requerido.")]
        public long CliCliente { get; set; }

        [Required(ErrorMessage = "La categoría de devolución es requerida.")]
        public long CtdCategoriaTipoDev { get; set; }

        [Required(ErrorMessage = "El motivo es requerido.")]
        [MaxLength(1000)]
        public string DevMotivo { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un ítem a devolver.")]
        public List<DevolucionDetalleCreateDto> Detalles { get; set; } = [];
    }

    // ── DTO Cambiar estado ───────────────────────────────────────────────────
    public class DevolucionUpdateEstadoDto
    {
        [Required(ErrorMessage = "El nuevo estado es requerido.")]
        public string DevEstado { get; set; } = string.Empty;
    }

    // ── DTO Respuesta completa ───────────────────────────────────────────────
    public class DevolucionResponseDto
    {
        public long DevDevolucion { get; set; }
        public long VenOrdenVenta { get; set; }
        public long CliCliente { get; set; }
        public long CtdCategoriaTipoDev { get; set; }
        public string? NombreCategoria { get; set; }
        public string DevNumeroRma { get; set; } = string.Empty;
        public string DevMotivo { get; set; } = string.Empty;
        public decimal DevMontoTotal { get; set; }
        public string DevEstado { get; set; } = string.Empty;
        public DateTime DevFechaCreacion { get; set; }
        public List<DevolucionDetalleResponseDto> Detalles { get; set; } = [];
    }

    // ── DTO Listado resumido ─────────────────────────────────────────────────
    public class DevolucionListDto
    {
        public long DevDevolucion { get; set; }
        public string DevNumeroRma { get; set; } = string.Empty;
        public long VenOrdenVenta { get; set; }
        public long CliCliente { get; set; }
        public string DevEstado { get; set; } = string.Empty;
        public decimal DevMontoTotal { get; set; }
        public DateTime DevFechaCreacion { get; set; }
        public string? NombreCategoria { get; set; }
    }

    // ── DTOs Categoría ───────────────────────────────────────────────────────
    public class CategoriaDevolucionResponseDto
    {
        public long CtdCategoriaTipoDev { get; set; }
        public string CtdCodigo { get; set; } = string.Empty;
        public string CtdNombre { get; set; } = string.Empty;
        public string? CtdDescripcion { get; set; }
        public string CtdEstado { get; set; } = string.Empty;
    }

    // ── DTOs Detalle ─────────────────────────────────────────────────────────
    public class DevolucionDetalleCreateDto
    {
        [Required(ErrorMessage = "El detalle de orden original es requerido.")]
        public long VdeOrdenVentaDetalle { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal DdeCantidad { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal DdeMonto { get; set; }
    }

    public class DevolucionDetalleResponseDto
    {
        public long DdeDevolucionDetalle { get; set; }
        public long VdeOrdenVentaDetalle { get; set; }
        public decimal DdeCantidad { get; set; }
        public decimal DdeMonto { get; set; }
        public string DdeEstado { get; set; } = string.Empty;
        public DateTime DdeFechaCreacion { get; set; }
    }
}
