using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos
{
    public interface IDepartamentoService
    {
        Task<ResponseSpDTO> CrearAsync(CreateDepartamentoDTO dto);
        Task<ResponseSpDTO> ActualizarAsync(int id, UpdateDepartamentoDTO dto);
        Task<ResponseDepartamentoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ResponseDepartamentoDTO>> ListarAsync();
    }
}
