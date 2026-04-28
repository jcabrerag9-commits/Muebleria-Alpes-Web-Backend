using MuebleriaAlpesWebBackend.Domain.DTOs.Common;
using MuebleriaAlpesWebBackend.Domain.DTOs.RecursosHumanos.Departamento;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _repository;

        public DepartamentoService(IDepartamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseSpDTO> CrearAsync(CreateDepartamentoDTO dto)
        {
            dto.Codigo = dto.Codigo.Trim().ToUpper();
            dto.Nombre = dto.Nombre.Trim();
            dto.Descripcion = dto.Descripcion?.Trim();

            return await _repository.CrearAsync(dto);
        }

        public async Task<ResponseSpDTO> ActualizarAsync(int id, UpdateDepartamentoDTO dto)
        {
            dto.Nombre = dto.Nombre.Trim();
            dto.Descripcion = dto.Descripcion?.Trim();

            return await _repository.ActualizarAsync(id, dto);
        }

        public async Task<ResponseDepartamentoDTO?> ObtenerPorIdAsync(int id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ResponseDepartamentoDTO>> ListarAsync()
        {
            return await _repository.ListarAsync();
        }
    }
}
