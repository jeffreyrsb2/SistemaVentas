using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Aplicacion.Servicios
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        // Inyectamos la INTERFAZ del repositorio, no la implementación concreta
        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<ProductoDto> ObtenerPorIdAsync(int id)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id);
            if (producto == null) return null;

            // Mapeamos de la Entidad de Dominio al DTO
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync()
        {
            var productos = await _productoRepository.ObtenerTodosAsync();

            // Mapeamos la lista de Entidades a una lista de DTOs
            return productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock
            });
        }

        public async Task<ProductoDto> CrearAsync(UpsertProductoDto productoDto)
        {
            var nuevoProducto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                Precio = productoDto.Precio,
                Stock = productoDto.Stock
            };
            var nuevoId = await _productoRepository.CrearAsync(nuevoProducto);

            // Devolvemos el objeto completo que se creó
            var productoCreado = await _productoRepository.ObtenerPorIdAsync(nuevoId);
            return new ProductoDto
            {
                Id = productoCreado.Id,
                Nombre = productoCreado.Nombre,
                Descripcion = productoCreado.Descripcion,
                Precio = productoCreado.Precio,
                Stock = productoCreado.Stock
            };
        }

        public async Task<bool> ActualizarAsync(int id, UpsertProductoDto productoDto)
        {
            var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);
            if (productoExistente == null) return false;

            productoExistente.Nombre = productoDto.Nombre;
            productoExistente.Descripcion = productoDto.Descripcion;
            productoExistente.Precio = productoDto.Precio;
            productoExistente.Stock = productoDto.Stock;

            return await _productoRepository.ActualizarAsync(productoExistente);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _productoRepository.EliminarAsync(id);
        }
    }
}
