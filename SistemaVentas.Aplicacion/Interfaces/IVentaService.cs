using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IVentaService { Task<VentaResponseDto> CrearVentaAsync(VentaRequestDto ventaDto, int usuarioId); }
}
