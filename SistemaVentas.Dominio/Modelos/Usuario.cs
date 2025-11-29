namespace SistemaVentas.Dominio.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public int RolId { get; set; }
    }
}
