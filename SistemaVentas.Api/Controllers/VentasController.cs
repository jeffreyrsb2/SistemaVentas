using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using System.Security.Claims;

namespace SistemaVentas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Vendedor")] // Solo Admin y Vendedor pueden acceder
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        public VentasController(IVentaService ventaService) { _ventaService = ventaService; }

        [HttpPost]
        public async Task<ActionResult<VentaResponseDto>> CrearVenta(VentaRequestDto ventaDto)
        {
            try
            {
                // Obtenemos el ID del usuario desde el token JWT
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var resultado = await _ventaService.CrearVentaAsync(ventaDto, usuarioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
