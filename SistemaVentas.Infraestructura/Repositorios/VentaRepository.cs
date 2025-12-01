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
    }
}
