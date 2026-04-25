using MuebleriaAlpesWebBackend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories
{
    public interface IClienteRepository
    {
        // Maestro
        Task<(int id, string codigo)> CrearClienteAsync(Cliente cliente);
        Task ActualizarClienteAsync(Cliente cliente);
        Task CambiarEstadoAsync(int clienteId, string nuevoEstado, string motivo, int? usuarioId);
        Task EliminarLogicoAsync(int clienteId, string motivo, int? usuarioId);
        Task<ClienteDetalleDto> GetDetalleAsync(int clienteId);
        Task<IEnumerable<Cliente>> ListarAsync();

        // Emails
        Task<int> AgregarEmailAsync(ClienteEmail email);
        Task ActualizarEmailAsync(ClienteEmail email);
        Task MarcarEmailPrincipalAsync(int emailId);
        Task EliminarEmailAsync(int emailId);

        // Teléfonos
        Task<int> AgregarTelefonoAsync(ClienteTelefono telefono);
        Task ActualizarTelefonoAsync(ClienteTelefono telefono);
        Task MarcarTelefonoPrincipalAsync(int telefonoId);
        Task EliminarTelefonoAsync(int telefonoId);

        // Direcciones
        Task<int> AgregarDireccionAsync(ClienteDireccion direccion);
        Task ActualizarDireccionAsync(ClienteDireccion direccion);
        Task MarcarDireccionPrincipalAsync(int direccionId);
        Task EliminarDireccionAsync(int direccionId);

        // Preferencias
        Task GuardarPreferenciasAsync(ClientePreferencia pref);
    }
}
