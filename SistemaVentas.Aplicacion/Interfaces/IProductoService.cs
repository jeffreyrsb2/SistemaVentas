using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IProductoService
    {
        Task<ProductoDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto> CrearAsync(UpsertProductoDto productoDto);
        Task<bool> ActualizarAsync(int id, UpsertProductoDto productoDto);
        Task<bool> EliminarAsync(int id);
    }
}
