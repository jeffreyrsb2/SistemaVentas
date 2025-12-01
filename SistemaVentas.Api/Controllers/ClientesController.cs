using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Aplicacion.DTOs; // TODO: Por ahora no usamos DTOs externos
using SistemaVentas.Dominio.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protegemos el controlador
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _clienteRepository.ObtenerTodosAsync();

            // Creamos el DTO aquí mismo para simplificar
            var clientesDto = clientes.Select(c => new
            {
                c.Id,
                c.NombreCompleto
            });

            return Ok(clientesDto);
        }
    }
}
