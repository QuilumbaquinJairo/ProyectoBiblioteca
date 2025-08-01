namespace BibliotecaBackend.DTOs.Chat
{
    public class RespuestaChatDto
    {
        public string Respuesta { get; set; } = string.Empty;
        public DateTime FechaRespuesta { get; set; } = DateTime.UtcNow;
        public bool EsDelSistema { get; set; } = true;
    }
}
