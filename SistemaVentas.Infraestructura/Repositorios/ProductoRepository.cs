using Microsoft.Extensions.Configuration;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public class ProductoRepository : RepositorioBase, IProductoRepository
    {
        public ProductoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            var productos = new List<Producto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerProductos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productos.Add(new Producto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock"))
                            });
                        }
                    }
                }
            }
            return productos;
        }

        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            Producto producto = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerProductoPorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            producto = new Producto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock"))
                            };
                        }
                    }
                }
            }
            return producto;
        }

        // --- TODO: Crear, Actualizar y Eliminar ---
        public Task<int> CrearAsync(Producto entidad) { throw new NotImplementedException(); }
        public Task<bool> ActualizarAsync(Producto entidad) { throw new NotImplementedException(); }
        public Task<bool> EliminarAsync(int id) { throw new NotImplementedException(); }
        public Task<IEnumerable<Producto>> ObtenerProductosPorDebajoDeStockMinimoAsync(int stockMinimo) { throw new NotImplementedException(); }
    }
}
