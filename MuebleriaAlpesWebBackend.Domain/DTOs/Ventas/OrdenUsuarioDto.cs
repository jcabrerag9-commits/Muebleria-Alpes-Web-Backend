namespace MuebleriaAlpesWebBackend.Domain.DTOs.Ventas
{
    /// <summary>
    /// Fila devuelta por el SYS_REFCURSOR de SP_LISTAR_ORDENES_USUARIO.
    /// Columnas: VEN_ORDEN_VENTA, VEN_NUMERO_ORDEN, VEN_FECHA_ORDEN, VEN_TOTAL, ESTADO_ORDEN
    /// </summary>
    public class OrdenUsuarioDto
    {
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public DateTime FechaOrden { get; set; }
        public decimal Total { get; set; }
        public string EstadoOrden { get; set; } = string.Empty;
    }

    /// <summary>
    /// Datos devueltos por SP_LISTAR_ORDENES_USUARIO (lista de órdenes).
    /// Este SP no retorna p_resultado/p_mensaje; se envuelve en BaseResponse manualmente en el Repository.
    /// </summary>
    public class ListarOrdenesUsuarioDataDto
    {
        public List<OrdenUsuarioDto> Ordenes { get; set; } = new();
    }
}
