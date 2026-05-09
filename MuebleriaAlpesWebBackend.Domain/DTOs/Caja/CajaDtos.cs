namespace MuebleriaAlpesWebBackend.Domain.DTOs.Caja
{
    public class AbrirCajaRequestDto
    {
        public decimal MontoInicial { get; set; }
    }

    public class CerrarCajaRequestDto
    {
        public int CorteId { get; set; }
        public decimal MontoFinal { get; set; }
        public string Observacion { get; set; } = string.Empty;
    }

    public class ConciliarCajaRequestDto
    {
        public int CorteId { get; set; }
        public string Observacion { get; set; } = string.Empty;
    }

    public class AbrirCajaDataDto
    {
        public int CorteId { get; set; }
    }
}
