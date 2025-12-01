using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public class ClienteRepository : RepositorioBase, IClienteRepository
    {
        public ClienteRepository(string connectionString) : base(connectionString) { }

        public async Task<Cliente> ObtenerPorIdAsync(int id)
        {
            Cliente cliente = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerClientePorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cliente = new Cliente
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                DocumentoIdentidad = reader.IsDBNull(reader.GetOrdinal("DocumentoIdentidad")) ? null : reader.GetString(reader.GetOrdinal("DocumentoIdentidad"))
                            };
                        }
                    }
                }
            }
            return cliente;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
        {
            var clientes = new List<Cliente>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerClientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clientes.Add(new Cliente
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                DocumentoIdentidad = reader.IsDBNull(reader.GetOrdinal("DocumentoIdentidad")) ? null : reader.GetString(reader.GetOrdinal("DocumentoIdentidad"))
                            });
                        }
                    }
                }
            }
            return clientes;
        }

        public Task<int> CrearAsync(Cliente cliente)
        {
            // TODO: Aún no hemos creado el Stored Procedure para esto.
            throw new NotImplementedException();
        }

        public Task<bool> ActualizarAsync(Cliente cliente)
        {
            // TODO: Aún no hemos creado el Stored Procedure para esto.
            throw new NotImplementedException();
        }

        public Task<bool> EliminarAsync(int id)
        {
            // TODO: Aún no hemos creado el Stored Procedure para esto.
            throw new NotImplementedException();
        }
    }
}
