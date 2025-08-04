using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.DTOs.Factura
{
    public class FacturaRequestDto
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int CodigoCiudadEntrega { get; set; }

        [Required]
        [StringLength(13)]
        public string RucCliente { get; set; }

        [Required]
        public List<FacturaDetalleDto> Detalles { get; set; }
    }
}
