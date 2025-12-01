using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Aplicacion.DTOs
{
    public class DetalleVentaDto { 
        public int ProductoId { get; set; } public string ProductoNombre { get; set; } public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } }
}
