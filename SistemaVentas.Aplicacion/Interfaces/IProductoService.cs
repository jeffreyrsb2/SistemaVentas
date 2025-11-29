using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IProductoService
    {
        Task<ProductoDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();
        // TODO: métodos de escritura
    }
}
