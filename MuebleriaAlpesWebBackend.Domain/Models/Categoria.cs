namespace MuebleriaAlpesWebBackend.Domain.Models
{
    /// <summary>
    /// Mapea ALP_CATEGORIA: CAT_CATEGORIA, CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION.
    /// La tabla no tiene columna de estado.
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
    }

    /// <summary>Producto devuelto al consultar los productos de una categoría.</summary>
    public class ProductoEnCategoria
    {
        public int ProductoId { get; set; }
        public string? Nombre { get; set; }
        public string? Sku { get; set; }
        public string? Estado { get; set; }
        public string? Publicado { get; set; }
    }
}
