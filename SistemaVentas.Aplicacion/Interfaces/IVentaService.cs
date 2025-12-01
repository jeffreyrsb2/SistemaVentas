using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IVentaService
    {
        Task<VentaResponseDto> CrearVentaAsync(VentaRequestDto ventaDto, string usuarioId);
        Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync();
    }
}
