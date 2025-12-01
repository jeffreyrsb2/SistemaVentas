using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IVentaRepository
    {
        Task<int> CrearAsync(Venta venta);
        Task<IEnumerable<Venta>> ObtenerTodasAsync();
        Task<Venta?> ObtenerPorIdAsync(int id);
        Task<bool> AnularAsync(int id);
    }
}
