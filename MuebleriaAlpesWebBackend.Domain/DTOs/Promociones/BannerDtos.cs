using System.ComponentModel.DataAnnotations;

namespace MuebleriaAlpesWebBackend.Domain.DTOs.Promociones
{
    public static class EstadoBanner
    {
        public const string Activo     = "ACTIVO";
        public const string Inactivo   = "INACTIVO";
        public const string Programado = "PROGRAMADO";
        public const string Expirado   = "EXPIRADO";
        public static readonly string[] Todos = [Activo, Inactivo, Programado, Expirado];
    }

    public static class PlataformaBanner
    {
        public const string Web    = "WEB";
        public const string Mobile = "MOBILE";
        public const string App    = "APP";
        public const string Tablet = "TABLET";
        public static readonly string[] Todas = [Web, Mobile, App, Tablet];
    }

    // ── DTO Crear banner ─────────────────────────────────────────────────────
    public class BannerCreateDto
    {
        [Required(ErrorMessage = "El título es requerido.")]
        [MaxLength(255)]
        public string BanTitulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL de la imagen es requerida.")]
        [MaxLength(500)]
        public string BanImagenUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? BanEnlace { get; set; }

        [MaxLength(50)]
        public string? BanPosicion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateTime BanFechaInicio { get; set; }

        public DateTime? BanFechaFin { get; set; }
    }

    // ── DTO Actualizar banner ────────────────────────────────────────────────
    public class BannerUpdateDto
    {
        [MaxLength(255)]
        public string? BanTitulo { get; set; }

        [MaxLength(500)]
        public string? BanImagenUrl { get; set; }

        [MaxLength(500)]
        public string? BanEnlace { get; set; }

        [MaxLength(50)]
        public string? BanPosicion { get; set; }

        public DateTime? BanFechaInicio { get; set; }
        public DateTime? BanFechaFin { get; set; }
        public string? BanEstado { get; set; }
    }

    // ── DTO Respuesta banner ─────────────────────────────────────────────────
    public class BannerResponseDto
    {
        public long BanBanner { get; set; }
        public string BanTitulo { get; set; } = string.Empty;
        public string BanImagenUrl { get; set; } = string.Empty;
        public string? BanEnlace { get; set; }
        public string? BanPosicion { get; set; }
        public DateTime BanFechaInicio { get; set; }
        public DateTime? BanFechaFin { get; set; }
        public string BanEstado { get; set; } = string.Empty;
        public bool EstaVigente =>
            BanEstado == EstadoBanner.Activo &&
            BanFechaInicio <= DateTime.Now &&
            (BanFechaFin == null || BanFechaFin >= DateTime.Now);
        public int TotalClicks { get; set; }
    }

    // ── DTO Registrar click ──────────────────────────────────────────────────
    public class ClickBannerCreateDto
    {
        public long? CliCliente { get; set; }

        [MaxLength(20)]
        public string? ClbPlataforma { get; set; }

        [MaxLength(100)]
        public string? ClbOrigen { get; set; }

        [MaxLength(500)]
        public string? ClbDetalle { get; set; }
    }

    // ── DTO Respuesta click ──────────────────────────────────────────────────
    public class ClickBannerResponseDto
    {
        public long ClbClickBanner { get; set; }
        public long BanBanner { get; set; }
        public long? CliCliente { get; set; }
        public DateTime ClbFechaClick { get; set; }
        public string? ClbPlataforma { get; set; }
        public string? ClbOrigen { get; set; }
    }

    // ── DTO Estadísticas de banner ───────────────────────────────────────────
    public class BannerEstadisticasDto
    {
        public long BanBanner { get; set; }
        public string BanTitulo { get; set; } = string.Empty;
        public int TotalClicks { get; set; }
        public int ClicksWeb { get; set; }
        public int ClicksMobile { get; set; }
        public int ClicksApp { get; set; }
        public int ClicksTablet { get; set; }
        public DateTime? UltimoClick { get; set; }
    }
}