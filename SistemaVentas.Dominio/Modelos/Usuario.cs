using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Dominio.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public int RolId { get; set; }
        [NotMapped] // Le dice a cualquier ORM que ignore esta propiedad
        public string RolNombre { get; set; }
    }
}
