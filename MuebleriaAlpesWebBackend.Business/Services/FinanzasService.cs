using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class FinanzasService : IFinanzasService
    {
        private readonly IFinanzasRepository _finanzasRepository;
        private readonly ILogger<FinanzasService> _logger;
        private readonly IConfiguration _configuration;

        public FinanzasService(IFinanzasRepository finanzasRepository, ILogger<FinanzasService> logger, IConfiguration configuration)
        {
            _finanzasRepository = finanzasRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<HistorialFinancieroDTO>> ObtenerHistorialFinancieroAsync(HistorialFiltroRequest request, System.Threading.CancellationToken ct = default)
        {
            // TODO REMOVE AFTER ERP STABILIZATION
            bool diagEnabled = _configuration["ERPDiagnostics:Enabled"]?.ToLower() == "true";
            var sw = diagEnabled ? Stopwatch.StartNew() : null;

            if (diagEnabled) _logger.LogInformation("[DIAGNOSTICO] Consulta Historial Financiero iniciada por Usuario {UsuarioId}", request.UsuarioId);

            var result = await _finanzasRepository.ObtenerHistorialFinancieroAsync(request, ct);

            if (sw != null) {
                sw.Stop();
                int threshold = int.TryParse(_configuration["ERPDiagnostics:SlowOperationThresholdMs"], out int t) ? t : 2000;
                if (sw.ElapsedMilliseconds > threshold)
                    _logger.LogWarning("[DIAGNOSTICO] CONSULTA LENTA: Historial Financiero tomó {Ms}ms", sw.ElapsedMilliseconds);
            }

            return result;
        }
    }
}
