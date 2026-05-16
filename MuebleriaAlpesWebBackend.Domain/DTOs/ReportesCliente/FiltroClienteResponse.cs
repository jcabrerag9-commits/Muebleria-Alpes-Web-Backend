namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente
{
    public class FiltroClienteResponse
    {
        public int ClienteId { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Documento { get; set; }
        public string? Estado { get; set; }
    }
}
