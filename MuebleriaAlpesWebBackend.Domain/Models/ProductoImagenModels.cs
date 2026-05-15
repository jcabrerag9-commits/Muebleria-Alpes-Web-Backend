using System;

namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class ProductoImagenDTO
    {
        public int ImagenId { get; set; }
        public int ProductoId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Tamanio { get; set; }
        public string? Url { get; set; }
        public string? Tipo { get; set; }
        public int Orden { get; set; }
        public byte[]? Archivo { get; set; }

        // Mapeo para nombres exactos de Oracle sp_obtener_imagen_producto
        public byte[]? PIM_ARCHIVO { get => Archivo; set => Archivo = value; }
        public string? PIM_CONTENT_TYPE { get => ContentType; set => ContentType = value; }
        public string? PIM_NOMBRE_ARCHIVO { get => NombreArchivo; set => NombreArchivo = value; }
        public long PIM_TAMANIO { get => Tamanio; set => Tamanio = value; }
        public int PRO_PRODUCTO { get => ProductoId; set => ProductoId = value; }

        public bool EsPrincipal => Tipo == "PRINCIPAL";
    }

    public class ProductoImagenListadoDTO
    {
        public int ImagenId { get; set; }
        public int ProductoId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Tamanio { get; set; }
        public string? Url { get; set; }
        public string? Tipo { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCarga { get; set; }
        public bool EsPrincipal => Tipo == "PRINCIPAL";
        public string ImagenUrl => $"/api/ProductoImagen/{ImagenId}";
    }
}
