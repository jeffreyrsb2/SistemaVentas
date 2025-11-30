using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;

namespace SistemaVentas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);
            if (loginResponse == null)
            {
                return Unauthorized(); // Devuelve 401 si el login falla
            }
            return Ok(loginResponse);
        }
    }
}
