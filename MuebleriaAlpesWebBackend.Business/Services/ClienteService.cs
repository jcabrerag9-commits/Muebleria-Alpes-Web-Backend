using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Business.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<int> RegistrarClienteAsync(Cliente cliente)
        {
            // Validaciones Senior
            if (string.IsNullOrWhiteSpace(cliente.NumeroDocumento))
                throw new ArgumentException("El número de documento es obligatorio.");

            // 1. Crear Maestro
            var (id, codigo) = await _clienteRepository.CrearClienteAsync(cliente);
            cliente.Id = id;
            cliente.Codigo = codigo;

            // 4. Crear Preferencias por defecto
            await _clienteRepository.GuardarPreferenciasAsync(new ClientePreferencia { ClienteId = id });

            return id;
        }

        public async Task<ClienteDetalleDto> GetPerfilAsync(int clienteId)
        {
            return await _clienteRepository.GetDetalleAsync(clienteId);
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.ListarAsync();
        }

        public async Task AddEmailAsync(ClienteEmail email) => await _clienteRepository.AgregarEmailAsync(email);
        public async Task AddTelefonoAsync(ClienteTelefono telefono) => await _clienteRepository.AgregarTelefonoAsync(telefono);
        public async Task AddDireccionAsync(ClienteDireccion direccion) => await _clienteRepository.AgregarDireccionAsync(direccion);
        public async Task UpdatePreferenciasAsync(ClientePreferencia pref) => await _clienteRepository.GuardarPreferenciasAsync(pref);

        public async Task ChangeStatusAsync(int clienteId, string nuevoEstado, string motivo, int? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe proporcionar un motivo para el cambio de estado.");

            await _clienteRepository.CambiarEstadoAsync(clienteId, nuevoEstado, motivo, usuarioId);
        }

        public async Task EliminarLogicoAsync(int clienteId, string motivo, int? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                motivo = "Eliminación desde sistema web";

            await _clienteRepository.EliminarLogicoAsync(clienteId, motivo, usuarioId);
        }
    }
}
