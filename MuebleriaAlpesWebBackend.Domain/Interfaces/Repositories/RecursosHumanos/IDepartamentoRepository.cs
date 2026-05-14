using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos
{
    public interface IDepartamentoRepository
    {
        Task<ResponseSpDTO> CrearAsync(CreateDepartamentoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateDepartamentoDTO dto);
        Task<ResponseDepartamentoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ResponseDepartamentoDTO>> ListarAsync();
    }
}
