namespace SistemaVentas.Infraestructura.Repositorios
{
    public abstract class RepositorioBase
    {
        protected readonly string _connectionString;

        public RepositorioBase(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
