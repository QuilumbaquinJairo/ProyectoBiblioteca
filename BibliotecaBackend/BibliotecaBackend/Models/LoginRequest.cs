using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Contrasenia { get; set; }
    }
}
