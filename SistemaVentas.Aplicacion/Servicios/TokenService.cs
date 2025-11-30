using Microsoft.IdentityModel.Tokens;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Dominio.Modelos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaVentas.Aplicacion.Servicios
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(string jwtKey, string issuer, string audience)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            _issuer = issuer;
            _audience = audience;
        }

        public string CrearToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.RolNombre)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
