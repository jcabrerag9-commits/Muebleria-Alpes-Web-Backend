namespace MuebleriaAlpesWebBackend.Domain.DTOs.Auth
{
    /// <summary>
    /// Respuesta del endpoint de diagnóstico de autenticación.
    /// Solo para uso en desarrollo — no exponer en producción.
    /// </summary>
    public class AuthDiagnosticoDto
    {
        public string Username          { get; set; } = string.Empty;
        public bool   UsuarioEncontrado { get; set; }
        public int    UsuarioId         { get; set; }
        public string EstadoUsuario     { get; set; } = string.Empty;
        public string HashGuardado        { get; set; } = string.Empty;
        public string HashCompleto        { get; set; } = string.Empty;   // hash completo para UPDATE
        public bool   BCryptOk            { get; set; }
        public bool   HashNuevoVerifica   { get; set; }                    // auto-validación del hash generado
        public string BCryptError         { get; set; } = string.Empty;
        public string HashGeneradoPrueba  { get; set; } = string.Empty;   // usar este en el UPDATE SQL
        public string RolAsignado       { get; set; } = string.Empty;
        public string Mensaje           { get; set; } = string.Empty;
    }
}
