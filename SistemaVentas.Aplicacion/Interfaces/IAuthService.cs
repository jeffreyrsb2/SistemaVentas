using SistemaVentas.Aplicacion.DTOs;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface IAuthService { Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest); }
}
