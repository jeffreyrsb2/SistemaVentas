namespace SistemaVentas.Dominio.Interfaces
{
    public interface IRepositorioGenerico<T> where T : class
    {
        Task<T> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<int> CrearAsync(T entidad);
        Task<bool> ActualizarAsync(T entidad);
        Task<bool> EliminarAsync(int id);
    }
}
