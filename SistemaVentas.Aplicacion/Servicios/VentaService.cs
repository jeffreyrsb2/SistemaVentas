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
        // Esta implementación es ineficiente (N+1), pero funciona para la demo.
        // En un escenario real, optimizaríamos esto en el repositorio.
        var ventas = await _ventaRepository.ObtenerTodasAsync();
        var dtos = new List<VentaResponseDto>();

        // Esto necesita que el sp_ObtenerVentas devuelva los IDs de cliente y usuario,
        // y que el VentaRepository los mapee.
        foreach (var venta in ventas)
        {
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

    public async Task<VentaDetalleResponseDto?> ObtenerPorIdAsync(int id)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(id);
        if (venta == null) return null;

        // Hacemos las llamadas adicionales para obtener los nombres
        var cliente = await _clienteRepository.ObtenerPorIdAsync(venta.ClienteId);
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(venta.UsuarioId);

        // Mapeamos la entidad a nuestro DTO de respuesta detallado
        var dto = new VentaDetalleResponseDto
        {
            Id = venta.Id,
            FechaVenta = venta.FechaVenta,
            Total = venta.Total,
            ClienteNombre = cliente?.NombreCompleto,
            UsuarioNombre = usuario?.NombreUsuario,
            Detalles = new List<DetalleVentaDto>()
        };

        foreach (var detalle in venta.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId);
            dto.Detalles.Add(new DetalleVentaDto
            {
                ProductoId = detalle.ProductoId,
                ProductoNombre = producto?.Nombre,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario
            });
        }

        return dto;
    }

    public async Task<bool> ActualizarVentaAsync(int id, VentaRequestDto ventaDto, string usuarioId)
    {
        var ventaExistente = await _ventaRepository.ObtenerPorIdAsync(id);
        if (ventaExistente == null) return false;

        decimal totalVenta = 0;
        var detallesVenta = new List<DetalleVenta>();
        var idUsuario = int.Parse(usuarioId);

        foreach (var item in ventaDto.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(item.ProductoId);
            if (producto == null) throw new Exception($"Producto con ID {item.ProductoId} no encontrado.");

            var stockOriginalItem = ventaExistente.Detalles.FirstOrDefault(d => d.ProductoId == item.ProductoId)?.Cantidad ?? 0;
            if ((producto.Stock + stockOriginalItem) < item.Cantidad)
            {
                throw new Exception($"Stock insuficiente para el producto '{producto.Nombre}'.");
            }

            totalVenta += producto.Precio * item.Cantidad;
            detallesVenta.Add(new DetalleVenta
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        var ventaActualizada = new Venta
        {
            Id = id,
            UsuarioId = idUsuario,
            ClienteId = ventaDto.ClienteId,
            Total = totalVenta,
            Detalles = detallesVenta
        };

        return await _ventaRepository.ActualizarAsync(ventaActualizada);
    }

    public async Task<bool> AnularAsync(int id)
    {
        return await _ventaRepository.AnularAsync(id);
    }
}
