namespace MuebleriaAlpesWebBackend.Domain.Entities
{
    /// <summary>
    /// Entidad que representa ALP_PROMOCION.
    /// Tipos válidos: PORCENTAJE, MONTO_FIJO, 2X1, ENVIO_GRATIS, COMPRA_MINIMA
    /// Estados válidos: ACTIVO, INACTIVO, EXPIRADO
    /// </summary>
    public class Promocion
    {
        public long PrmPromocion { get; set; }
        public string PrmCodigo { get; set; } = string.Empty;
        public string PrmNombre { get; set; } = string.Empty;
        public string? PrmDescripcion { get; set; }
        public string PrmTipo { get; set; } = string.Empty;
        public decimal? PrmValor { get; set; }
        public DateTime PrmFechaInicio { get; set; }
        public DateTime PrmFechaFin { get; set; }
        public string PrmEstado { get; set; } = "ACTIVO";
    }

    /// <summary>
    /// Entidad que representa ALP_PROMOCION_PRODUCTO.
    /// Relaciona una promoción con un producto base O una variante (nunca ambos).
    /// Estados válidos: ACTIVO, INACTIVO
    /// </summary>
    public class PromocionProducto
    {
        public long PpoPromocionProducto { get; set; }
        public long PrmPromocion { get; set; }
        public long? ProProducto { get; set; }
        public long? PvaProductoVariante { get; set; }
        public string PpoEstado { get; set; } = "ACTIVO";
        public DateTime PpoFechaCreacion { get; set; }
    }
}
