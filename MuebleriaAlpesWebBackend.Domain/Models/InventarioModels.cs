using System;
using System.Collections.Generic;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    // --- WRAPPERS ---
    public class InventarioResponse<T>
    {
        public string Resultado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSuccess => Resultado == "EXITO";
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
        public int CantidadDisponible { get; set; }
        public int CantidadReservada { get; set; }
        public DateTime UltimaActualizacion { get; set; }
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
}
