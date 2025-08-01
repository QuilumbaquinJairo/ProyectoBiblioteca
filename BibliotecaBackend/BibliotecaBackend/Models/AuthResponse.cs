namespace BibliotecaBackend.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
    }
}
