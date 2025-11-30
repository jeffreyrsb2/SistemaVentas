using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<int> CrearAsync(Producto entidad);
        Task<bool> ActualizarAsync(Producto entidad);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<Producto>> ObtenerProductosPorDebajoDeStockMinimoAsync(int stockMinimo);
    }
}
