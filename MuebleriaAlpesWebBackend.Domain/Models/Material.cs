namespace MuebleriaAlpesWebBackend.Domain.Models
{
    /// <summary>
    /// Mapea ALP_MATERIAL: MAT_MATERIAL, MAT_CODIGO, MAT_NOMBRE, MAT_DESCRIPCION, MAT_ESTADO
    /// </summary>
    public class Material
    {
        public int     Id          { get; set; }
        public string? Codigo      { get; set; }
        public string  Nombre      { get; set; } = "";
        public string? Descripcion { get; set; }
        public string  Estado      { get; set; } = "ACTIVO";
    }
}
