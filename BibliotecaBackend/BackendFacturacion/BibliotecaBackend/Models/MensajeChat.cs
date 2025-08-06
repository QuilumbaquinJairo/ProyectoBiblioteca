namespace BibliotecaBackend.Models
{
    public class MensajeChat
    {
        public int Id { get; set; }
        public int SesionChatId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool EsDelUsuario { get; set; } = true;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

        public SesionChat SesionChat { get; set; } = null!;
    }
}
