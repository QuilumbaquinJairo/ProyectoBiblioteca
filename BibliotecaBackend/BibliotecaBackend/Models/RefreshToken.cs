using BibliotecaBackend.Data;

namespace BibliotecaBackend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string CreadoPorIp { get; set; } = string.Empty;
        public DateTime? FechaRevocacion { get; set; }
        public string? RevocadoPorIp { get; set; }
        public string? TokenReemplazo { get; set; }

        public int UsuarioId { get; set; } 
        public Usuario Usuario { get; set; } = null!;

        public bool EstaActivo => FechaRevocacion == null && !EstaExpirado;
        public bool EstaExpirado => DateTime.UtcNow >= FechaExpiracion;
    }
}
