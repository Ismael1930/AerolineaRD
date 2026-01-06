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
        public string? Clase { get; set; } // "Economica", "Ejecutiva", "Primera"
        public string? TipoVuelo { get; set; } // ? AGREGADO: "SoloIda", "IdaYVuelta"
        public DateTime? FechaRegreso { get; set; } // ? AGREGADO: Solo para vuelos de ida y vuelta
        public List<int>? IdsTripulacion { get; set; }
    }

    public class ActualizarVueloDto
    {
        public int Id { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public TimeSpan? HoraLlegada { get; set; }
        public int? Duracion { get; set; }
        public decimal? PrecioBase { get; set; }
        public string? OrigenCodigo { get; set; }
        public string? DestinoCodigo { get; set; }
        public string? Matricula { get; set; } // ? Matrícula de la aeronave (PK)
        public string? Estado { get; set; }
        public string? TipoVuelo { get; set; } // "SoloIda", "IdaYVuelta"
        public string? Clase { get; set; }
        public DateTime? FechaRegreso { get; set; } // ? AGREGADO: Solo para vuelos de ida y vuelta
    }

    public class VueloDetalleDto : VueloResponseDto
    {
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public int Duracion { get; set; }
        public decimal PrecioBase { get; set; }
        public string? Matricula { get; set; }
        public string? Estado { get; set; }
        public string? Clase { get; set; }
        public List<TripulacionDto>? Tripulacion { get; set; }
        public EstadoVueloDto? EstadoActual { get; set; }
        public AeronaveInfoDto? Aeronave { get; set; } // Información de la aeronave
    }

    /// <summary>
    /// DTO para mostrar vuelos con información completa de la aeronave asignada
    /// </summary>
    public class VueloConAeronaveDto
    {
        // Información del Vuelo
        public int Id { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public string? Origen { get; set; }
        public string? Destino { get; set; }
        public string? Estado { get; set; }
        public string? Clase { get; set; }

        // Información de la Aeronave
        public AeronaveInfoDto? Aeronave { get; set; }
    }

    public class AeronaveInfoDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
        public int TiempoPreparacionMinutos { get; set; }
        public int TotalAsientos { get; set; }
    }
}