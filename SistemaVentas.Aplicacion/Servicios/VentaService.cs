using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    // TODO: Asumimos que también tenemos un IClienteRepository
    // private readonly IClienteRepository _clienteRepository;

    public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository)
    {
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
    }

    public async Task<VentaResponseDto> CrearVentaAsync(VentaRequestDto ventaDto, int usuarioId)
    {
        // --- Lógica de Negocio ---
        decimal totalVenta = 0;
        var detallesVenta = new List<DetalleVenta>();

        foreach (var item in ventaDto.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
            if (producto == null) throw new Exception($"Producto con ID {item.ProductoId} no encontrado.");
            if (producto.Stock < item.Cantidad) throw new Exception($"Stock insuficiente para el producto '{producto.Nombre}'.");

            totalVenta += producto.Precio * item.Cantidad;
            detallesVenta.Add(new DetalleVenta
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        var nuevaVenta = new Venta
        {
            UsuarioId = usuarioId,
            ClienteId = ventaDto.ClienteId,
            Total = totalVenta,
            Detalles = detallesVenta
        };

        var ventaCreada = await _ventaRepository.CrearAsync(nuevaVenta);

        return new VentaResponseDto { Id = ventaCreada.Id, Total = ventaCreada.Total, FechaVenta = ventaCreada.FechaVenta };
    }
}
