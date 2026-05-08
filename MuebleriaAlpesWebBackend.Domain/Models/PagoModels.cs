namespace MuebleriaAlpesWebBackend.Domain.Models
{
    public class ProcesarPagoRequest
    {
        public int OrdenId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal Monto { get; set; }
        public int MonedaId { get; set; }
        public string Referencia { get; set; } = string.Empty;
    }

    public class ProcesarPagoResponse
    {
        public int? PagoId { get; set; }
        public int? FacturaId { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool IsSuccess => Resultado == "EXITO";
    }
}
