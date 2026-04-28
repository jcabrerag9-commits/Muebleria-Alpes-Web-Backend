using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Empleado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface IEmpleadoRepository
    {
        Task<ResponseSpDTO> CrearAsync(CreateEmpleadoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateEmpleadoDTO dto);
        Task<ResponseSpDTO> CambiarEstadoAsync(int id, CambiarEstadoEmpleadoDTO dto);
        Task<ResponseEmpleadoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ResponseEmpleadoDTO>> ListarAsync(string? estado);
    }
}
