namespace AerolineaRD.Data.DTOs
{
    /// <summary>
    /// DTO para obtener información de una ruta
    /// </summary>
    public class RutaDto
    {
        public int Id { get; set; }
        public string OrigenCodigo { get; set; } = null!;
        public string DestinoCodigo { get; set; } = null!;
        public int DuracionMinutos { get; set; }
        public int? DistanciaKm { get; set; }
        public bool Activa { get; set; }
        
        // Información adicional calculada
        public string? DuracionFormato { get; set; } // Ej: "2h 30m"
        
        // Información de los aeropuertos
        public string? OrigenNombre { get; set; }
        public string? OrigenCiudad { get; set; }
        public string? DestinoNombre { get; set; }
        public string? DestinoCiudad { get; set; }
    }

    /// <summary>
    /// DTO para consultar la duración de una ruta específica
    /// </summary>
    public class ConsultarRutaDto
    {
        public string OrigenCodigo { get; set; } = null!;
        public string DestinoCodigo { get; set; } = null!;
    }

    /// <summary>
    /// DTO para la respuesta de duración de ruta (usado en el frontend)
    /// Incluye cálculos automáticos de hora de llegada y precio sugerido
    /// </summary>
    public class RutaDuracionDto
    {
        public string OrigenCodigo { get; set; } = null!;
        public string DestinoCodigo { get; set; } = null!;
        public int DuracionMinutos { get; set; }
        public string DuracionFormato { get; set; } = null!; // Ej: "2h 30m"
        public TimeSpan Duracion { get; set; } // Para cálculos en el frontend
        public bool RutaEncontrada { get; set; }
        public string? Mensaje { get; set; }
        
        // ? NUEVO: Hora de llegada calculada (si se proporcionó hora de salida)
        public TimeSpan? HoraLlegadaCalculada { get; set; }
        public string? HoraLlegadaFormato { get; set; } // Ej: "2:30 PM"
        
        /// <summary>
        /// Indica si el vuelo cruza la medianoche (llega al día siguiente)
        /// </summary>
        public bool CruzaMedianoche { get; set; }
        
        /// <summary>
        /// Mensaje informativo si cruza medianoche
        /// </summary>
        public string? NotaMedianoche { get; set; }
        
        // ? NUEVO: Precio sugerido basado en duración
        public decimal? PrecioSugerido { get; set; }
        public string? PrecioFormato { get; set; } // Ej: "$450.00"
        
        // ? NUEVO: Desglose de precios por clase
        public PreciosPorClaseDto? PreciosPorClase { get; set; }
        
        // ? NUEVO: Información adicional de la ruta
        public int? DistanciaKm { get; set; }
        public string? TipoRuta { get; set; } // "Nacional", "Regional", "Internacional", "Intercontinental"
    }

    /// <summary>
    /// DTO para precios calculados por clase
    /// </summary>
    public class PreciosPorClaseDto
    {
        public decimal Economica { get; set; }
        public decimal Ejecutiva { get; set; }
        public decimal Primera { get; set; }
        
        public string EconomicaFormato { get; set; } = null!;
        public string EjecutivaFormato { get; set; } = null!;
        public string PrimeraFormato { get; set; } = null!;
    }

    /// <summary>
    /// DTO para crear una nueva ruta
    /// </summary>
    public class CrearRutaDto
    {
        public string OrigenCodigo { get; set; } = null!;
        public string DestinoCodigo { get; set; } = null!;
        public int DuracionMinutos { get; set; }
        public int? DistanciaKm { get; set; }
    }

    /// <summary>
    /// DTO para actualizar una ruta existente
    /// </summary>
    public class ActualizarRutaDto
    {
        public int Id { get; set; }
        public int? DuracionMinutos { get; set; }
        public int? DistanciaKm { get; set; }
        public bool? Activa { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de horas disponibles en un aeropuerto
    /// </summary>
    public class HorasDisponiblesDto
    {
        public string OrigenCodigo { get; set; } = null!;
        public string DestinoCodigo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string FechaFormato { get; set; } = null!;
        
        /// <summary>
        /// Lista de horas disponibles para crear vuelos
        /// </summary>
        public List<HoraDisponibleDto> HorasDisponibles { get; set; } = new();
        
        /// <summary>
        /// Lista de horas ocupadas (para referencia)
        /// </summary>
        public List<HoraOcupadaDto> HorasOcupadas { get; set; } = new();
        
        /// <summary>
        /// Capacidad máxima del aeropuerto por hora
        /// </summary>
        public int CapacidadPorHora { get; set; }
        
        /// <summary>
        /// Nombre del aeropuerto de origen
        /// </summary>
        public string? OrigenNombre { get; set; }
        
        /// <summary>
        /// Información de la ruta (duración, precio, etc)
        /// </summary>
        public RutaDuracionDto? InfoRuta { get; set; }
        
        public string? Mensaje { get; set; }
    }

    /// <summary>
    /// DTO para una hora disponible
    /// </summary>
    public class HoraDisponibleDto
    {
        /// <summary>
        /// Hora en formato TimeSpan (para cálculos)
        /// </summary>
        public TimeSpan Hora { get; set; }
        
        /// <summary>
        /// Hora formateada para mostrar (ej: "10:00 AM")
        /// </summary>
        public string HoraFormato { get; set; } = null!;
        
        /// <summary>
        /// Valor para el input (ej: "10:00")
        /// </summary>
        public string Valor { get; set; } = null!;
        
        /// <summary>
        /// Hora de llegada calculada si se selecciona esta hora
        /// </summary>
        public TimeSpan? HoraLlegada { get; set; }
        
        /// <summary>
        /// Hora de llegada formateada
        /// </summary>
        public string? HoraLlegadaFormato { get; set; }
        
        /// <summary>
        /// Indica si el vuelo cruza medianoche
        /// </summary>
        public bool CruzaMedianoche { get; set; }
        
        /// <summary>
        /// Espacios disponibles en esta hora (capacidad - ocupados)
        /// </summary>
        public int EspaciosDisponibles { get; set; }
    }

    /// <summary>
    /// DTO para una hora ocupada
    /// </summary>
    public class HoraOcupadaDto
    {
        public TimeSpan Hora { get; set; }
        public string HoraFormato { get; set; } = null!;
        public int VuelosProgramados { get; set; }
        public int CapacidadMaxima { get; set; }
        public bool Saturada { get; set; }
        public List<string>? VuelosEnHora { get; set; } // Números de vuelo
    }
}
