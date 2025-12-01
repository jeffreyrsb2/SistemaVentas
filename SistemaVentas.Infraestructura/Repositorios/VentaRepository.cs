using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public class VentaRepository : RepositorioBase, IVentaRepository
    {
        public VentaRepository(string connectionString) : base(connectionString) { }

        public async Task<int> CrearAsync(Venta venta)
        {
            var detallesTable = new DataTable();
            detallesTable.Columns.Add("ProductoId", typeof(int));
            detallesTable.Columns.Add("Cantidad", typeof(int));
            detallesTable.Columns.Add("PrecioUnitario", typeof(decimal));

            foreach (var detalle in venta.Detalles)
            {
                detallesTable.Rows.Add(detalle.ProductoId, detalle.Cantidad, detalle.PrecioUnitario);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_CrearVenta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UsuarioId", venta.UsuarioId);
                    command.Parameters.AddWithValue("@ClienteId", venta.ClienteId);
                    command.Parameters.AddWithValue("@Total", venta.Total);

                    var detallesParam = command.Parameters.AddWithValue("@Detalles", detallesTable);
                    detallesParam.SqlDbType = SqlDbType.Structured;
                    detallesParam.TypeName = "dbo.DetalleVentaType";

                    // El SP devuelve el ID
                    var nuevaVentaId = (int)await command.ExecuteScalarAsync();
                    return nuevaVentaId;
                }
            }
        }

        public async Task<IEnumerable<Venta>> ObtenerTodasAsync()
        {
            var ventas = new List<Venta>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerVentas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ventas.Add(new Venta
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId"))
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public async Task<Venta?> ObtenerPorIdAsync(int id)
        {
            Venta venta = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ObtenerVentaPorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Leer el primer resultado (la cabecera de la venta)
                        if (await reader.ReadAsync())
                        {
                            venta = new Venta
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId"))
                            };
                        }

                        // Avanzar al segundo resultado (los detalles)
                        if (venta != null && await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                venta.Detalles.Add(new DetalleVenta
                                {
                                    ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                                    Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                                    // Nota: El ProductoNombre lo ignoramos aquí, lo usará el Service.
                                });
                            }
                        }
                    }
                }
            }
            return venta;
        }

        public async Task<bool> ActualizarAsync(Venta venta)
        {
            var detallesTable = new DataTable();
            detallesTable.Columns.Add("ProductoId", typeof(int));
            detallesTable.Columns.Add("Cantidad", typeof(int));
            detallesTable.Columns.Add("PrecioUnitario", typeof(decimal));

            foreach (var detalle in venta.Detalles)
            {
                detallesTable.Rows.Add(detalle.ProductoId, detalle.Cantidad, detalle.PrecioUnitario);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ActualizarVenta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", venta.Id);
                    command.Parameters.AddWithValue("@UsuarioId", venta.UsuarioId);
                    command.Parameters.AddWithValue("@ClienteId", venta.ClienteId);
                    command.Parameters.AddWithValue("@Total", venta.Total);

                    var detallesParam = command.Parameters.AddWithValue("@Detalles", detallesTable);
                    detallesParam.SqlDbType = SqlDbType.Structured;
                    detallesParam.TypeName = "dbo.DetalleVentaType";

                    // Usamos ExecuteScalarAsync para leer la señal de éxito (el '1')
                    var resultado = await command.ExecuteScalarAsync();

                    // Si el resultado no es nulo y es 1, la transacción fue exitosa.
                    return resultado != null && Convert.ToInt32(resultado) == 1;
                }
            }
        }

        public async Task<bool> AnularAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_AnularVenta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", id);
                    var filasAfectadas = await command.ExecuteNonQueryAsync();
                    return filasAfectadas > 0;
                }
            }
        }
    }
}
