namespace MuebleriaAlpesWebBackend.Domain.DTOs.Carrito
{
    /// <summary>
    /// Cabecera del carrito devuelta por el primer SYS_REFCURSOR de SP_OBTENER_CARRITO_CLIENTE.
    /// Columnas: CAR_CARRITO, CAR_SUBTOTAL, CAR_FECHA_CREACION
    /// </summary>
    public class CarritoCabeceraDto
    {
        public int CarritoId { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// Línea de detalle del carrito devuelta por el segundo SYS_REFCURSOR de SP_OBTENER_CARRITO_CLIENTE.
    /// Columnas: CAD_CARRITO_DETALLE, PRO_PRODUCTO, PRO_NOMBRE, CAD_CANTIDAD, CAD_PRECIO_UNITARIO, CAD_SUBTOTAL
    /// </summary>
    public class CarritoDetalleDto
    {
        public int DetalleId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Datos compuestos devueltos por SP_OBTENER_CARRITO_CLIENTE (cabecera + detalles).
    /// Este SP no retorna p_resultado/p_mensaje; se envuelve en BaseResponse manualmente en el Repository.
    /// </summary>
    public class ObtenerCarritoClienteDataDto
    {
        public CarritoCabeceraDto? Cabecera { get; set; }
        public List<CarritoDetalleDto> Detalles { get; set; } = new();
    }
}
