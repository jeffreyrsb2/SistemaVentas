using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public class UsuarioRepository : RepositorioBase, IUsuarioRepository
    {
        public UsuarioRepository(string connectionString) : base(connectionString) { }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            Usuario usuario = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerUsuarioPorNombre", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                RolId = reader.GetInt32(reader.GetOrdinal("RolId")),
                                RolNombre = reader.GetString(reader.GetOrdinal("RolNombre"))
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        // TODO: Implementar este método si es necesario en el futuro
        public Task<Usuario?> ObtenerPorIdAsync(int id) => throw new NotImplementedException();
    }
}
