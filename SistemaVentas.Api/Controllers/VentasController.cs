using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Protegemos todo el controlador
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;
    public VentasController(IVentaService ventaService) { _ventaService = ventaService; }

    [HttpPost]
    [Authorize(Roles = "Administrador,Vendedor")] // Solo estos roles pueden crear
    public async Task<ActionResult<VentaResponseDto>> CrearVenta(VentaRequestDto ventaDto)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId == null) return Unauthorized();

            var resultado = await _ventaService.CrearVentaAsync(ventaDto, usuarioId);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")] // Solo el admin puede ver todas las ventas
    public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetVentas()
    {
        var ventas = await _ventaService.ObtenerTodasAsync();
        return Ok(ventas);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<VentaDetalleResponseDto>> GetVenta(int id)
    {
        var venta = await _ventaService.ObtenerPorIdAsync(id);
        if (venta == null) return NotFound();
        return Ok(venta);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AnularVenta(int id)
    {
        var resultado = await _ventaService.AnularAsync(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}
