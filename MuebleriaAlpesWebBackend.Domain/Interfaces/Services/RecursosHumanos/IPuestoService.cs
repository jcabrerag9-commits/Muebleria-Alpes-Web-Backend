using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Puesto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface IPuestoService
    {
        Task<ResponseSpDTO> CrearAsync(CreatePuestoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdatePuestoDTO dto);
        Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoPuestoDTO dto);
        Task<ResponsePuestoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ResponsePuestoDTO>> ListarAsync(bool soloActivos);
    }
}
