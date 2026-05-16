using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly ILogger<PagoService> _logger;
        private readonly IConfiguration _configuration;

        public PagoService(
            IPagoRepository pagoRepository, 
            ILogger<PagoService> logger,
            IConfiguration configuration)
        {
            _pagoRepository = pagoRepository;
            _logger = logger;
            _configuration = configuration;
        }

        // CAMBIO ERP 2026-05-11: Motor centralizado de traducción de errores Oracle
        private string MapearErrorOracle(string mensajeOracle)
        {
            if (string.IsNullOrEmpty(mensajeOracle)) return "Error interno desconocido";
            
            if (mensajeOracle.Contains("ORA-20050")) return "La orden no fue encontrada para facturación.";
            if (mensajeOracle.Contains("ORA-20051")) return "Tipo de comprobante no configurado en el sistema.";
            if (mensajeOracle.Contains("ORA-20060")) return "El pago excede el saldo pendiente de la factura o la orden está cancelada.";
            if (mensajeOracle.Contains("ORA-20061")) return "Error al emitir la factura electrónica vinculada al pago.";
            if (mensajeOracle.Contains("ORA-20062")) return "La forma de pago seleccionada no es válida.";
            if (mensajeOracle.Contains("ORA-20063")) return "Moneda de pago inválida.";
            if (mensajeOracle.Contains("ORA-20064")) return "La orden no existe o está en uso por otro usuario.";
            if (mensajeOracle.Contains("ORA-20065")) return "No se pueden recibir pagos sobre una factura ANULADA.";
            if (mensajeOracle.Contains("ORA-20066")) return "La factura ya se encuentra totalmente PAGADA en el ERP.";
            if (mensajeOracle.Contains("ORA-20080")) return "El pago ya se encuentra anulado o en un estado inválido.";
            if (mensajeOracle.Contains("ORA-20081")) return "No se puede anular un pago vinculado a un cierre contable (CONCILIADO).";
            
            return "Ocurrió un error en el ERP. Por favor contacte soporte técnico.";
        }

        public async Task<ProcesarPagoResponse> ProcesarPagoAsync(ProcesarPagoRequest request, System.Threading.CancellationToken ct = default)
        {
            // TODO REMOVE AFTER ERP STABILIZATION
            bool diagEnabled = _configuration["ERPDiagnostics:Enabled"]?.ToLower() == "true";
            var sw = diagEnabled ? Stopwatch.StartNew() : null;

            try
            {
                if (diagEnabled) 
                    _logger.LogInformation("[DIAGNOSTICO] Iniciando Pago: Orden={OrdenId}, Monto={Monto}, Usuario={UsuarioId}", request.OrdenId, request.Monto, request.UsuarioId);

                // CAMBIO ERP 2026-05-11: Oracle maneja todo el procesamiento interno (FOR UPDATE, Estados, Kardex).
                var response = await _pagoRepository.ProcesarPagoAsync(request, null, ct);

                if (sw != null) {
                    sw.Stop();
                    int threshold = int.TryParse(_configuration["ERPDiagnostics:SlowOperationThresholdMs"], out int t) ? t : 2000;
                    if (sw.ElapsedMilliseconds > threshold)
                        _logger.LogWarning("[DIAGNOSTICO] OPERACIÓN LENTA: Pago Orden {OrdenId} tomó {Ms}ms", request.OrdenId, sw.ElapsedMilliseconds);
                }

                if (response.IsSuccess && response.FacturaId.HasValue)
                {
                    // Operación exitosa. Oracle ya actualizó el estado de la factura y el Kardex.
                }
                else if (!response.IsSuccess && !string.IsNullOrEmpty(response.Mensaje))
                {
                    if (response.Mensaje.Contains("ORA-"))
                    {
                        response.Mensaje = MapearErrorOracle(response.Mensaje);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico ERP procesando el pago para la Orden {OrdenId}", request.OrdenId);
                return new ProcesarPagoResponse 
                { 
                    Success = false,
                    Resultado = "ERROR", 
                    Mensaje = ex.Message.Contains("ORA-") ? MapearErrorOracle(ex.Message) : "Error inesperado de comunicación con Oracle."
                };
            }
        }

        public async Task<PagoDTO?> ObtenerPorIdAsync(int id)
        {
            return await _pagoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<PagoDTO>> ObtenerTodosAsync()
        {
            return await _pagoRepository.ObtenerTodosAsync();
        }

        public async Task<IEnumerable<PagoDTO>> ObtenerPorFacturaIdAsync(int facturaId)
        {
            return await _pagoRepository.ObtenerPorFacturaIdAsync(facturaId);
        }

        public async Task<ApiResponse<bool>> AnularPagoAsync(int id, string motivo, int usuarioId, System.Threading.CancellationToken ct = default)
        {
            return await _pagoRepository.AnularPagoAsync(id, motivo, usuarioId, ct);
        }

        public async Task<IEnumerable<OrdenPendientePagoDTO>> ObtenerOrdenesPendientesAsync()
        {
            return await _pagoRepository.ObtenerOrdenesPendientesAsync();
        }
    }
}
