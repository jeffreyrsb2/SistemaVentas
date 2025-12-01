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
}
