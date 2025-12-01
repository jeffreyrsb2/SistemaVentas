using System.Data;
using System.Data.SqlClient;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Infraestructura.Repositorios
{
    public class VentaRepository : RepositorioBase, IVentaRepository
    {
        public VentaRepository(string connectionString) : base(connectionString) { }

        public async Task<Venta> CrearAsync(Venta venta)
        {
            // Creamos un DataTable en memoria que coincide con nuestro TYPE de SQL
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

                    // Pasamos el DataTable como un parámetro especial
                    var detallesParam = command.Parameters.AddWithValue("@Detalles", detallesTable);
                    detallesParam.SqlDbType = SqlDbType.Structured;
                    detallesParam.TypeName = "dbo.DetalleVentaType";

                    // El SP devuelve el ID de la nueva venta
                    var nuevaVentaId = (int)await command.ExecuteScalarAsync();
                    venta.Id = nuevaVentaId;
                    return venta;
                }
            }
        }

        // TODO:El resto de los métodos por ahora
        public Task<Venta?> ObtenerPorIdConDetallesAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Venta>> ObtenerTodasConDetallesAsync() => throw new NotImplementedException();
    }
}
