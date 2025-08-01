using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaBackend.Models
{
    public class Usuario
    {
        public Usuario()
        {
            Id = 0;
            NombreUsuario = string.Empty;
            Correo = string.Empty;
            ContrasenaHash = string.Empty;

        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string ContrasenaHash { get; set; }

        [Required]
        public string Rol { get; set; } = "Usuario"; // Usuario, Administrador, etc.

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
