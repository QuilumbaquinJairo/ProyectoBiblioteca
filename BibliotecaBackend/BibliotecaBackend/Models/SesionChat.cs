using BibliotecaBackend.Data;

namespace BibliotecaBackend.Models
{
    public class SesionChat
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }
        public string PantallaContexto { get; set; } = string.Empty;
        public bool EstaActiva { get; set; } = true;

        public Usuario Usuario { get; set; } = null!;
        public List<MensajeChat> Mensajes { get; set; } = new();
    }
}
