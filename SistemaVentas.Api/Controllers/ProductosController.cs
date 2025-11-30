using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;

namespace SistemaVentas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);

            if (producto == null)
            {
                return NotFound(); // Devuelve 404 si no se encuentra
            }

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto(UpsertProductoDto productoDto)
        {
            var nuevoProducto = await _productoService.CrearAsync(productoDto);
            return CreatedAtAction(nameof(GetProducto), new { id = nuevoProducto.Id }, nuevoProducto);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, UpsertProductoDto productoDto)
        {
            var resultado = await _productoService.ActualizarAsync(id, productoDto);
            if (!resultado) return NotFound();
            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var resultado = await _productoService.EliminarAsync(id);
            if (!resultado) return NotFound();
            return NoContent();
        }
    }
}
