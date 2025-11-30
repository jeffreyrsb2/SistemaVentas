using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IClienteRepository
    {
        Task<Producto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<int> CrearAsync(Producto entidad);
        Task<bool> ActualizarAsync(Producto entidad);
        Task<bool> EliminarAsync(int id);
    }
}
