namespace SistemaVentas.Dominio.Modelos
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public int UsuarioId { get; set; }
        public int ClienteId { get; set; }
        public bool Eliminado { get; set; }

        // Propiedad de navegación para los detalles
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
