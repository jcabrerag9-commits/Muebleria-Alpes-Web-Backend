namespace MuebleriaAlpesWebBackend.Domain.DTOs.Envios
{
    public class FiltroDireccionClienteResponse
    {
        public int ClienteDireccionId { get; set; }
        public int ClienteId { get; set; }
        public int CiudadId { get; set; }
        public string? CiudadNombre { get; set; }
        public string? Tipo { get; set; }
        public string? DireccionLinea1 { get; set; }
        public string? DireccionLinea2 { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Referencia { get; set; }
        public string? Principal { get; set; }
        public string? Estado { get; set; }
    }
}
