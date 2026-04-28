using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Nomina;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface INominaService
    {
        Task<ResponseSpDTO> CrearAsync(CrearNominaDTO dto);
        Task<ResponseSpDTO> CambiarEstadoAsync(int nominaId, CambiarEstadoNominaDTO dto);
        Task<NominaResponseDTO?> ObtenerPorIdAsync(int nominaId);
        Task<IEnumerable<NominaResponseDTO>> ListarAsync(string? estado);

        Task<ResponseSpDTO> AgregarEmpleadoAsync(int nominaId, AgregarEmpleadoNominaDTO dto);
        Task<ResponseSpDTO> CalcularDetalleAsync(int detalleId, CalcularNominaDetalleDTO dto);
        Task<IEnumerable<NominaDetalleResponseDTO>> ListarDetalleAsync(int nominaId);

        Task<ResponseSpDTO> AgregarIngresoAsync(int detalleId, AgregarIngresoNominaDTO dto);
        Task<IEnumerable<NominaIngresoResponseDTO>> ListarIngresosAsync(int detalleId);

        Task<ResponseSpDTO> AgregarDeduccionAsync(int detalleId, AgregarDeduccionNominaDTO dto);
        Task<IEnumerable<NominaDeduccionResponseDTO>> ListarDeduccionesAsync(int detalleId);
    }
}
