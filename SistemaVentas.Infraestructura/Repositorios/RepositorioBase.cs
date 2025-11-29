using Microsoft.Extensions.Configuration;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public abstract class RepositorioBase
    {
        protected readonly string _connectionString;

        public RepositorioBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
