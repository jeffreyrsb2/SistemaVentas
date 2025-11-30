using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Aplicacion.DTOs
{
    public class UpsertProductoDto
    {
        [Required]
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
