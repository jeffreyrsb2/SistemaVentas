using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Dominio.Interfaces
{
    public interface IUsuarioRepository
    {
        // Los usuarios requieren tratamiento especial, no se usa la interfaz genérica para tener más control
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task<Usuario?> ObtenerPorIdAsync(int id);
    }
}
