namespace SistemaVentas.Aplicacion.DTOs
{
    public class VentaRequestDto { public int ClienteId { get; set; } public List<DetalleVentaRequestDto> Detalles { get; set; } }
}
