using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public VentaService(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository)
    {
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<VentaResponseDto> CrearVentaAsync(VentaRequestDto ventaDto, string usuarioId)
    {
        // --- Lógica de Negocio ---
        decimal totalVenta = 0;
        var detallesVenta = new List<DetalleVenta>();
        var idUsuario = int.Parse(usuarioId);

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
            UsuarioId = idUsuario,
            ClienteId = ventaDto.ClienteId,
            Total = totalVenta,
            Detalles = detallesVenta
        };

        var ventaCreadaId = await _ventaRepository.CrearAsync(nuevaVenta);

        // Para devolver una respuesta completa, buscamos los datos recién creados
        var cliente = await _clienteRepository.ObtenerPorIdAsync(ventaDto.ClienteId);
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario);

        return new VentaResponseDto
        {
            Id = ventaCreadaId,
            Total = nuevaVenta.Total,
            FechaVenta = DateTime.UtcNow, // Aproximación
            ClienteNombre = cliente?.NombreCompleto,
            UsuarioNombre = usuario?.NombreUsuario
        };
    }

    public async Task<IEnumerable<VentaResponseDto>> ObtenerTodasAsync()
    {
        // Esto es ineficiente y es un ejemplo de "N+1 query problem"
        // pero demuestra la separación de capas.
        // En un escenario real, usaríamos joins o consultas optimizadas.
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        var dtos = new List<VentaResponseDto>();

        foreach (var venta in ventas)
        {
            // Por cada venta, hacemos 2 llamadas más a la BD para obtener los nombres.
            var cliente = await _clienteRepository.ObtenerPorIdAsync(venta.ClienteId);
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(venta.UsuarioId);

            dtos.Add(new VentaResponseDto
            {
                Id = venta.Id,
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                ClienteNombre = cliente?.NombreCompleto ?? "N/A",
                UsuarioNombre = usuario?.NombreUsuario ?? "N/A"
            });
        }
        return dtos;
    }
}
