using SistemaVentas.Dominio.Modelos;

namespace SistemaVentas.Aplicacion.Interfaces
{
    public interface ITokenService { string CrearToken(Usuario usuario); }
}
