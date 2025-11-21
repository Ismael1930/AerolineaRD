namespace AerolineaRD.Data.DTOs
{
    public class NotificacionResponseDto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leida { get; set; }
    }

    public class MarcarNotificacionLeidaDto
    {
        public int IdNotificacion { get; set; }
    }
}