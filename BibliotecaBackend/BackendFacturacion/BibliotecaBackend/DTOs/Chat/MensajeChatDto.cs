namespace BibliotecaBackend.DTOs.Chat
{
    public class MensajeChatDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public string PantallaContexto { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public bool EsDelUsuario { get; set; } = true;
    }
}
