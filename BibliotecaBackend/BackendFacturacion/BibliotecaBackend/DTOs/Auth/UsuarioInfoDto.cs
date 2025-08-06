namespace BibliotecaBackend.DTOs.Auth
{
    public class UsuarioInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool EstaActivo { get; set; }
    }
}
