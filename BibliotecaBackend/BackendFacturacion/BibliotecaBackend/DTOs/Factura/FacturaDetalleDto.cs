using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.DTOs.Factura
{
    public class FacturaDetalleDto
    {
        [Required]
        public int CodigoArticulo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }
    }
}
