namespace MuebleriaAlpesWebBackend.Domain.Entities
{
    /// <summary>
    /// Banner publicitario - ALP_BANNER.
    /// Estados válidos: ACTIVO, INACTIVO, PROGRAMADO, EXPIRADO
    /// Posiciones válidas: HOME_PRINCIPAL, HOME_SECUNDARIO, CATEGORIA, PRODUCTO, LATERAL
    /// </summary>
    public class Banner
    {
        public long BanBanner { get; set; }
        public string BanTitulo { get; set; } = string.Empty;
        public string BanImagenUrl { get; set; } = string.Empty;
        public string? BanEnlace { get; set; }
        public string? BanPosicion { get; set; }
        public DateTime BanFechaInicio { get; set; }
        public DateTime? BanFechaFin { get; set; }
        public string BanEstado { get; set; } = "PROGRAMADO";
    }

    /// <summary>
    /// Tracking de clicks en banners - ALP_CLICK_BANNER.
    /// Registra cada interacción de un cliente con un banner.
    /// Plataformas válidas: WEB, MOBILE, APP, TABLET
    /// </summary>
    public class ClickBanner
    {
        public long ClbClickBanner { get; set; }
        public long BanBanner { get; set; }
        public long? CliCliente { get; set; }
        public DateTime ClbFechaClick { get; set; }
        public string? ClbPlataforma { get; set; }
        public string? ClbOrigen { get; set; }
        public string? ClbDetalle { get; set; }
    }
}