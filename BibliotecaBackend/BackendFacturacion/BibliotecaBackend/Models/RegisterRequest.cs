using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.Models
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [MinLength(6)]
        public string Contrasenia { get; set; }

        public string Rol { get; set; } = "Usuario";
    }
}
