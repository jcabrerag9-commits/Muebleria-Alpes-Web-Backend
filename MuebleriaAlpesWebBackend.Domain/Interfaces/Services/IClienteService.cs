using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        Task<int> RegistrarClienteAsync(Cliente cliente);
        Task<ClienteDetalleDto> GetPerfilAsync(int clienteId);
        Task<IEnumerable<Cliente>> GetAllAsync();
        
        // Operaciones de detalle
        Task AddEmailAsync(ClienteEmail email);
        Task AddTelefonoAsync(ClienteTelefono telefono);
        Task AddDireccionAsync(ClienteDireccion direccion);
        Task UpdatePreferenciasAsync(ClientePreferencia pref);
        
        Task ChangeStatusAsync(int clienteId, string nuevoEstado, string motivo, int? usuarioId);
        Task EliminarLogicoAsync(int clienteId, string motivo, int? usuarioId);
    }
}
