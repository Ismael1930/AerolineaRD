namespace AerolineaRD.Data.DTOs
{
    public class AeropuertoDto
    {
        public string Codigo { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
    }

    /// <summary>
    /// DTO para mostrar la capacidad y uso de un aeropuerto
    /// </summary>
    public class AeropuertoCapacidadDto
    {
        public string Codigo { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public int CapacidadPorHora { get; set; }

        // Estadísticas generales
        public int TotalVuelosSalida { get; set; }
        public int TotalVuelosLlegada { get; set; }
        public int TotalVuelos { get; set; }

        // Uso por hora (para mostrar en gráficos)
        public List<UsoHorarioDto> UsoPorHora { get; set; } = new();

        // Porcentaje de ocupación
        public decimal PorcentajeUsoSalidas { get; set; }
        public decimal PorcentajeUsoLlegadas { get; set; }
        public decimal PorcentajeUsoTotal { get; set; }
    }

    /// <summary>
    /// DTO para mostrar el uso de un aeropuerto en una hora específica
    /// </summary>
    public class UsoHorarioDto
    {
        public int Hora { get; set; } // 0-23
        public string HoraFormato { get; set; } = null!; // "08:00 - 09:00"
        public int VuelosSalida { get; set; }
        public int VuelosLlegada { get; set; }
        public int TotalVuelos { get; set; }
        public int CapacidadDisponibleSalida { get; set; }
        public int CapacidadDisponibleLlegada { get; set; }
        public decimal PorcentajeUsoSalida { get; set; }
        public decimal PorcentajeUsoLlegada { get; set; }
        public bool SobreCapacidadSalida { get; set; }
        public bool SobreCapacidadLlegada { get; set; }
    }

    /// <summary>
    /// DTO para el reporte de capacidad de todos los aeropuertos
    /// </summary>
    public class ReporteCapacidadAeropuertosDto
    {
        public DateTime FechaConsulta { get; set; }
        public List<AeropuertoCapacidadDto> Aeropuertos { get; set; } = new();
        public int TotalAeropuertos { get; set; }
        public int AeropuertosSobreCapacidad { get; set; }
    }
}