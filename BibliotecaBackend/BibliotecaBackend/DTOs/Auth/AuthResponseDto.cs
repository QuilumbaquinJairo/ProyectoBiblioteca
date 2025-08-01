namespace BibliotecaBackend.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiracion { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
