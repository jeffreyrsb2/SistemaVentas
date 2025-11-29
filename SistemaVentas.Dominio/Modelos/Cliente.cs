namespace SistemaVentas.Dominio.Modelos
{
    public class Cliente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string? DocumentoIdentidad { get; set; }
    }
}
