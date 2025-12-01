namespace SistemaVentas.Aplicacion.DTOs
{
    public class VentaDetalleResponseDto : VentaResponseDto { 
        public List<DetalleVentaDto> Detalles { get; set; } 
    }
}
