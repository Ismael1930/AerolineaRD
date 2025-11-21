namespace AerolineaRD.Data.DTOs
{
    public class CrearVueloDto
    {
        public string? NumeroVuelo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public int Duracion { get; set; }
        public decimal PrecioBase { get; set; }
        public string? OrigenCodigo { get; set; }
        public string? DestinoCodigo { get; set; }
        public string? Matricula { get; set; }
        public List<int>? IdsTripulacion { get; set; }
    }

    public class ActualizarVueloDto
    {
        public int IdVuelo { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public TimeSpan? HoraLlegada { get; set; }
        public decimal? PrecioBase { get; set; }
        public string? Estado { get; set; }
    }

    public class VueloDetalleDto : VueloResponseDto
    {
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public int Duracion { get; set; }
        public decimal PrecioBase { get; set; }
        public string? Matricula { get; set; }
        public string? Estado { get; set; }
        public List<TripulacionDto>? Tripulacion { get; set; }
        public EstadoVueloDto? EstadoActual { get; set; }
    }
}