using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IProductoRepository : IRepositorioGenerico<Producto>
    {
        Task<IEnumerable<Producto>> ObtenerProductosPorDebajoDeStockMinimoAsync(int stockMinimo);
    }
}
