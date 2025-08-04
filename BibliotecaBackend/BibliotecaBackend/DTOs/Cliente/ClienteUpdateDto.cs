using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.DTOs.Cliente
{
    public class ClienteUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(200)]
        public string Direccion { get; set; }
    }
}
