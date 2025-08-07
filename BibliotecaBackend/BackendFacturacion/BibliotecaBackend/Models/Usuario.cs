using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.Models
{
    public class UsuarioChat
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Rol { get; set; } = "Usuario";  // Usuario, Admin, Soporte, etc.

        public bool Activo { get; set; } = true;
    }
}
