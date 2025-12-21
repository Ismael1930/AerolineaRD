namespace AerolineaRD.Data.DTOs
{
    public class CrearAeronaveDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    public class ActualizarAeronaveDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int? Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    public class AeronaveResponseDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    /// <summary>
    /// DTO extendido con información de disponibilidad de asientos
    /// </summary>
    public class AeronaveConDisponibilidadDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
        public int TiempoPreparacionMinutos { get; set; }

        // Información de asientos
        public int TotalAsientos { get; set; }
        public DisponibilidadAsientosDto DisponibilidadAsientos { get; set; } = new();

        // Estadísticas de vuelos
        public int TotalVuelosProgramados { get; set; }
        public int VuelosHoy { get; set; }
    }

    /// <summary>
    /// DTO para mostrar disponibilidad de asientos por clase
    /// </summary>
    public class DisponibilidadAsientosDto
    {
        // Primera Clase
        public int PrimeraTotal { get; set; }
        public int PrimeraReservados { get; set; }
        public int PrimeraDisponibles { get; set; }
        public decimal PrimeraPorcentajeOcupacion { get; set; }

        // Ejecutiva
        public int EjecutivaTotal { get; set; }
        public int EjecutivaReservados { get; set; }
        public int EjecutivaDisponibles { get; set; }
        public decimal EjecutivaPorcentajeOcupacion { get; set; }

        // Economica
        public int EconomicaTotal { get; set; }
        public int EconomicaReservados { get; set; }
        public int EconomicaDisponibles { get; set; }
        public decimal EconomicaPorcentajeOcupacion { get; set; }

        // Totales
        public int Total { get; set; }
        public int TotalReservados { get; set; }
        public int TotalDisponibles { get; set; }
        public decimal PorcentajeOcupacionTotal { get; set; }
    }
}