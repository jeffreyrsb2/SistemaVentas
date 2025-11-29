using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IVentaRepository
    {
        Task<Venta> CrearAsync(Venta venta);
        Task<Venta?> ObtenerPorIdConDetallesAsync(int id);
        Task<IEnumerable<Venta>> ObtenerTodasConDetallesAsync();
    }
}
