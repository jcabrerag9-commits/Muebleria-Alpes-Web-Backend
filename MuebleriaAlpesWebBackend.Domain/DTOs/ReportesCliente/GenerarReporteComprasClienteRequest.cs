namespace MuebleriaAlpesWebBackend.Domain.DTOs.ReportesCliente
{
    public class GenerarReporteComprasClienteRequest : ReporteClienteBaseRequest
    {
        public int? UsuarioId { get; set; }
    }
}