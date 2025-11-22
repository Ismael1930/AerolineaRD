namespace AerolineaRD.Data.DTOs
{
    public class VueloResponseDto
    {
        public int Id { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public int Duracion { get; set; } // Duración en minutos
        public decimal PrecioBase { get; set; } // Precio base económico
        public string? OrigenCodigo { get; set; }
        public string? OrigenNombre { get; set; }
        public string? OrigenCiudad { get; set; }
        public string? DestinoCodigo { get; set; }
        public string? DestinoNombre { get; set; }
        public string? DestinoCiudad { get; set; }
        public string? TipoVuelo { get; set; } // "SoloIda" o "IdaYVuelta"
        public DateTime? FechaRegreso { get; set; } // Solo para vuelos de ida y vuelta
     
        // Lista de clases disponibles con sus precios y disponibilidad
        public List<ClaseDisponibilidadDto> ClasesDisponibles { get; set; } = new();
    }
}