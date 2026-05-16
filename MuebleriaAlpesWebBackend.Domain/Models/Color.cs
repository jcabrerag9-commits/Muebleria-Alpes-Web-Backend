namespace MuebleriaAlpesWebBackend.Domain.Models
{
    /// <summary>
    /// Mapea ALP_COLOR: COL_COLOR, COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_DESCRIPCION, COL_ESTADO
    /// </summary>
    public class Color
    {
        public int     Id         { get; set; }
        public string? Codigo     { get; set; }
        public string  Nombre     { get; set; } = "";
        public string? HexColor   { get; set; }
        public string? Descripcion{ get; set; }
        public string  Estado     { get; set; } = "ACTIVO";
    }
}
