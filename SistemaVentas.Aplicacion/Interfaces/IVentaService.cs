using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IVentaService
    {
        Task<VentaResponseDto> CrearVentaAsync(VentaRequestDto ventaDto, string usuarioId);
        Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync();
        Task<VentaDetalleResponseDto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarVentaAsync(int id, VentaRequestDto ventaDto, string usuarioId);
        Task<bool> AnularAsync(int id);
    }
}
