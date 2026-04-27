namespace MuebleriaAlpesWebBackend.Domain.Entities
{
    /// <summary>
    /// Catálogo de motivos de devolución - ALP_CATEGORIA_TIPO_DEVOLUCION.
    /// Ejemplos: DEFECTO_FABRICA, PRODUCTO_INCORRECTO, ARREPENTIMIENTO
    /// </summary>
    public class CategoriaDevolucion
    {
        public long CtdCategoriaTipoDev { get; set; }
        public string CtdCodigo { get; set; } = string.Empty;
        public string CtdNombre { get; set; } = string.Empty;
        public string? CtdDescripcion { get; set; }
        public string CtdEstado { get; set; } = "ACTIVO";
    }

    /// <summary>
    /// Solicitud de devolución RMA - ALP_DEVOLUCION.
    /// Estados: SOLICITADA → EN_REVISION → APROBADA → COMPLETADA
    ///          SOLICITADA → RECHAZADA  |  EN_REVISION → RECHAZADA
    /// </summary>
    public class Devolucion
    {
        public long DevDevolucion { get; set; }
        public long VenOrdenVenta { get; set; }
        public long CliCliente { get; set; }
        public long CtdCategoriaTipoDev { get; set; }
        public string DevNumeroRma { get; set; } = string.Empty;
        public string DevMotivo { get; set; } = string.Empty;
        public decimal DevMontoTotal { get; set; }
        public string DevEstado { get; set; } = "SOLICITADA";
        public DateTime DevFechaCreacion { get; set; }

        // Para joins con la categoría
        public string? NombreCategoria { get; set; }
    }

    /// <summary>
    /// Ítem devuelto dentro de una devolución - ALP_DEVOLUCION_DETALLE.
    /// Estados válidos: PENDIENTE, RECIBIDO, INSPECCIONADO, REINTEGRADO
    /// </summary>
    public class DevolucionDetalle
    {
        public long DdeDevolucionDetalle { get; set; }
        public long DevDevolucion { get; set; }
        public long VdeOrdenVentaDetalle { get; set; }
        public decimal DdeCantidad { get; set; }
        public decimal DdeMonto { get; set; }
        public string DdeEstado { get; set; } = "PENDIENTE";
        public DateTime DdeFechaCreacion { get; set; }
    }
}
