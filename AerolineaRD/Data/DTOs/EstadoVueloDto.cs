namespace AerolineaRD.Data.DTOs
{
    public class ActualizarEstadoVueloDto
    {
        public int IdVuelo { get; set; }
        public string? Estado { get; set; }
        public DateTime? HoraSalida { get; set; }
        public DateTime? HoraLlegada { get; set; }
        public string? Puerta { get; set; }
        public string? Observaciones { get; set; }
    }

    public class EstadoVueloDto
    {
        public int Id { get; set; }
        public int IdVuelo { get; set; }
        public string? Estado { get; set; }
        public DateTime? HoraSalida { get; set; }
        public DateTime? HoraLlegada { get; set; }
        public DateTime HoraSalidaProgramada { get; set; }
        public DateTime HoraLlegadaProgramada { get; set; }
        public string? Puerta { get; set; }
        public string? Observaciones { get; set; }
    }
}