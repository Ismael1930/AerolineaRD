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
}
