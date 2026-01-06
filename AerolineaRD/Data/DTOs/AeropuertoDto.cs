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
    /// DTO para mostrar la capacidad y uso de un aeropuerto en formato calendario
    /// </summary>
    public class AeropuertoCapacidadDto
    {
        public string Codigo { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
 
        /// <summary>
        /// ? Capacidad máxima de vuelos por hora
        /// </summary>
        public int CapacidadPorHora { get; set; }
    
        /// <summary>
        /// Capacidad máxima de vuelos (salidas + llegadas) que puede manejar el aeropuerto por día
        /// </summary>
        public int CapacidadDiaria { get; set; }

        /// <summary>
        /// Estadísticas generales del período consultado
        /// </summary>
        public int TotalDiasConVuelos { get; set; }
        public int TotalVuelosSalida { get; set; }
        public int TotalVuelosLlegada { get; set; }
        public int TotalVuelos { get; set; }

        /// <summary>
        /// Calendario de días con vuelos programados (solo días con actividad)
        /// </summary>
        public List<UsoDiarioDto> DiasConVuelos { get; set; } = new();

        /// <summary>
        /// Porcentaje de uso promedio
        /// </summary>
        public decimal PorcentajeUsoPromedio { get; set; }
     
        /// <summary>
        /// Número de días que superan la capacidad
        /// </summary>
        public int DiasSobreCapacidad { get; set; }
    }

    /// <summary>
    /// DTO para mostrar el uso de un aeropuerto en un día específico (celda del calendario)
    /// </summary>
    public class UsoDiarioDto
    {
        public DateTime Fecha { get; set; }
        public string FechaFormato { get; set; } = null!; // "Lun, 15 Ene 2025"
        public int DiaSemana { get; set; } // 0=Domingo, 1=Lunes, ...
        public string NombreDiaSemana { get; set; } = null!; // "Lunes", "Martes", ...
        
        public int VuelosSalida { get; set; }
        public int VuelosLlegada { get; set; }
        public int TotalVuelos { get; set; }
        
        public int CapacidadDiaria { get; set; }
        public int CapacidadDisponible { get; set; }
        
        /// <summary>
        /// Porcentaje de uso del día (0-100, puede superar 100 si hay sobrecapacidad)
        /// </summary>
        public decimal PorcentajeUso { get; set; }
        
        /// <summary>
        /// Indica si el aeropuerto está sobre capacidad este día
        /// </summary>
        public bool SobreCapacidad { get; set; }
        
        /// <summary>
        /// Nivel de alerta: "BAJO" (0-60%), "MEDIO" (61-85%), "ALTO" (86-100%), "CRITICO" (>100%)
        /// </summary>
        public string NivelAlerta { get; set; } = null!;

        /// <summary>
        /// ? NUEVO: Desglose de uso por hora para este día específico
        /// </summary>
        public List<UsoHorarioDto> UsoPorHora { get; set; } = new();
    }

    /// <summary>
    /// ? NUEVO: DTO para mostrar el uso de un aeropuerto en una hora específica de un día
    /// </summary>
    public class UsoHorarioDto
    {
        public int Hora { get; set; } // 0-23
        public string HoraFormato { get; set; } = null!; // "08:00 - 08:59"
        
        public int VuelosSalida { get; set; }
        public int VuelosLlegada { get; set; }
        public int TotalVuelos { get; set; }
        
        public int CapacidadPorHora { get; set; }
        public int CapacidadDisponibleHora { get; set; }
        
        public decimal PorcentajeUsoHora { get; set; }
        public bool SobreCapacidadHora { get; set; }
    }

    /// <summary>
 /// DTO para el reporte de capacidad de todos los aeropuertos
    /// </summary>
    public class ReporteCapacidadAeropuertosDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalDias { get; set; }
        public List<AeropuertoCapacidadDto> Aeropuertos { get; set; } = new();
        public int TotalAeropuertos { get; set; }
        public int AeropuertosSobreCapacidad { get; set; }
    }
}