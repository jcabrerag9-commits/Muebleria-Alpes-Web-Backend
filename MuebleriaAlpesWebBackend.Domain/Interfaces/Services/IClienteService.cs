using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        Task<int> RegistrarClienteAsync(Cliente cliente, ClienteEmail emailInicial, ClienteTelefono telInicial);
        Task<ClienteDetalleDto> GetPerfilAsync(int clienteId);
        Task<IEnumerable<Cliente>> GetAllAsync();
        
        // Operaciones de detalle
        Task<int> AddEmailAsync(ClienteEmail email);
        Task<int> AddTelefonoAsync(ClienteTelefono telefono);
        Task<int> AddDireccionAsync(ClienteDireccion direccion);
        Task UpdatePreferenciasAsync(ClientePreferencia pref);
        
        Task ChangeStatusAsync(int clienteId, string nuevoEstado, string motivo, int? usuarioId);
    }
}
