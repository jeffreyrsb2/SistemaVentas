namespace SistemaVentas.Aplicacion.DTOs
{
    public class VentaConDetallesDto
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string ClienteNombre { get; set; }
        public string UsuarioNombre { get; set; }
    }
}
