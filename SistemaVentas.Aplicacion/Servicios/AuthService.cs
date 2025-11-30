using SistemaVentas.Aplicacion.DTOs;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Dominio.Interfaces;

namespace SistemaVentas.Aplicacion.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(loginRequest.NombreUsuario);

            // Verificamos si el usuario existe y si la contraseña es correcta usando BCrypt
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.PasswordHash))
            {
                return null; // Falló la autenticación
            }

            var token = _tokenService.CrearToken(usuario);
            return new LoginResponseDto { Token = token };
        }
    }
}
