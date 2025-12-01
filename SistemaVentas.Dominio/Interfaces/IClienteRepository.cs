using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IClienteRepository
    {
        // Métodos que necesitaremos para la funcionalidad de Clientes.
        Task<Cliente> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();

        // TODO: Aunque no los implementemos ahora en el repositorio,
        // los definimos en el contrato para un futuro.
        Task<int> CrearAsync(Cliente cliente);
        Task<bool> ActualizarAsync(Cliente cliente);
        Task<bool> EliminarAsync(int id);
    }
}
