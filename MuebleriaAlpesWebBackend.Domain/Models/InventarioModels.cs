using System;
using System.Collections.Generic;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    // --- WRAPPERS ---
    /// <summary>
    /// Wrapper legado para inventario, ahora basado en el estándar ApiResponse (H.4)
    /// </summary>
    public class InventarioResponse<T> : ApiResponse<T>
    {
    }

    // --- PRODUCTO DTOs ---
    public class ProductoDTO
    {
        public int ProductoId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string DescripcionCorta { get; set; } = string.Empty;
        public string DescripcionLarga { get; set; } = string.Empty;
        public decimal? Peso { get; set; }
        public string EsConfigurable { get; set; } = "N";
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        
        public int? TipoMuebleId { get; set; }
        public string? TipoMuebleNombre { get; set; }
        public string ImagenUrl => $"/api/ProductoImagen/producto/{ProductoId}/principal";
    }

    public class CrearProductoRequest
    {
        public int TipoMueble { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? DescripcionCorta { get; set; }
        public string? DescripcionLarga { get; set; }
        public decimal? Peso { get; set; }
        public string EsConfigurable { get; set; } = "N";
    }

    public class ActualizarProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? DescripcionCorta { get; set; }
        public decimal? Peso { get; set; }
        public int? TipoMueble { get; set; }
    }

    // --- VARIANTE DTOs ---
    public class VarianteDTO
    {
        public int VarianteId { get; set; }
        public int ProductoId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? CodigoBarras { get; set; }
        public string? ImagenUrl { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class CrearVarianteRequest
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? CodigoBarras { get; set; }
        public string? ImagenUrl { get; set; }
    }

    // --- INVENTARIO DTOs ---
    public class MovimientoInventarioRequest
    {
        public int ProductoId { get; set; }
        public int? VarianteId { get; set; }
        public int BodegaId { get; set; }
        public int Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? OrdenVentaId { get; set; }
        public string? Observacion { get; set; }
        public int? UsuarioId { get; set; }
    }

    public class ReservaStockRequest
    {
        public int ProductoId { get; set; }
        public int? VarianteId { get; set; }
        public int BodegaId { get; set; }
        public int? ClienteId { get; set; }
        public int Cantidad { get; set; }
        public string Motivo { get; set; } = "CARRITO";
        public DateTime? Expiracion { get; set; }
        public int? UsuarioId { get; set; }
    }

    public class ExistenciaDTO
    {
        public int ProductoId { get; set; }
        public int? VarianteId { get; set; }
        public int BodegaId { get; set; }
        public string? NombreBodega { get; set; }
        // Cantidad libre para nuevas ventas (Available to Promise)
        public int CantidadDisponible { get; set; }
        // Suma agregada de todas las reservas activas sobre este registro (no individual)
        public int CantidadReservada { get; set; }
        // Total fisico = Disponible + Reservado
        public int TotalFisico => CantidadDisponible + CantidadReservada;
        public DateTime UltimaActualizacion { get; set; }
    }

    /// <summary>
    /// Representa una reserva individual de stock.
    /// Motivo posibles: CARRITO | APARTADO_MANUAL | ORDEN_CONFIRMADA | RESERVA_TEMPORAL
    /// Estado posibles: ACTIVA | LIBERADA | EXPIRADA
    /// </summary>
    public class ReservaDTO
    {
        public int ReservaId { get; set; }
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; }
        public int? VarianteId { get; set; }
        public int BodegaId { get; set; }
        public string? NombreBodega { get; set; }
        public int? ClienteId { get; set; }
        public int Cantidad { get; set; }
        public string Motivo { get; set; } = "RESERVA_TEMPORAL";
        public string Estado { get; set; } = "ACTIVO";
        public DateTime? Expiracion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// Catalogo de bodegas para poblar selectores en la UI.
    /// </summary>
    public class BodegaDTO
    {
        public int? BodegaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = "ACTIVO";
        public string? Ubicacion { get; set; }
        
        // Nuevos campos ERP
        public string Tipo { get; set; } = "FISICA"; // FISICA | VIRTUAL
        public int? CanalVentaId { get; set; }
        public string? CanalVentaNombre { get; set; }
        public string PermiteReserva { get; set; } = "S";
        public string PermiteVenta { get; set; } = "S";
        public string ManejaDespacho { get; set; } = "S";
        public string? MotivoCierre { get; set; }
        
        // Estadísticas (solo lectura)
        public int StockTotal { get; set; }
        public int ReservasActivas { get; set; }
    }

    public class AjusteStockRequest
    {
        public int ProductoId { get; set; }
        public int? VarianteId { get; set; }
        public int BodegaId { get; set; }
        public int CantidadNueva { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    /// <summary>
    /// Representa un registro histórico en el Kardex de inventario.
    /// </summary>
    public class KardexDTO
    {
        public int MovimientoId { get; set; }
        public int ProductoId { get; set; }
        public string? Sku { get; set; }
        public string? NombreProducto { get; set; }
        public int BodegaId { get; set; }
        public string? NombreBodega { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int? StockAnterior { get; set; }
        public int? StockNuevo { get; set; }
        public int? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public DateTime Fecha { get; set; }
        public string? Observacion { get; set; }
        public int? OrdenVentaId { get; set; }
    }

    public class MovimientoFiltroRequest
    {
        public int? ProductoId { get; set; }
        public int? BodegaId { get; set; }
        public int? UsuarioId { get; set; }
        public int? TipoMovimientoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? TipoMovimiento { get; set; }
        public int? OrdenVentaId { get; set; }
    }
}
