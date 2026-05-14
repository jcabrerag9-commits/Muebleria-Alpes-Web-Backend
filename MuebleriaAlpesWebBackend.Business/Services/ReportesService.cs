using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ReportesService : IReportesService
    {
        private readonly IReportesRepository _reportesRepository;

        public ReportesService(IReportesRepository reportesRepository)
        {
            _reportesRepository = reportesRepository;
        }

        public async Task<BaseResponse> RegistrarEjecucionReporteAsync(RegistrarEjecucionReporteRequestDto request)
        {
            if (request.UsuarioId <= 0)
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El ID del usuario debe ser mayor a cero."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NombreReporte))
            {
                return new BaseResponse
                {
                    Resultado = "ERROR",
                    Mensaje = "El nombre del reporte es requerido."
                };
            }

            return await _reportesRepository.RegistrarEjecucionReporteAsync(request);
        }
    }
}
