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

    /// <summary>
    /// DTO para la respuesta de aeronaves disponibles para un horario específico
    /// </summary>
    public class AeronavesDisponiblesResponseDto
    {
        /// <summary>
        /// Aeronaves disponibles para el horario solicitado
        /// </summary>
        public List<AeronaveDisponibleDto> Disponibles { get; set; } = new();

        /// <summary>
        /// Aeronaves no disponibles con la razón
        /// </summary>
        public List<AeronaveNoDisponibleDto> NoDisponibles { get; set; } = new();

        /// <summary>
        /// Parámetros de búsqueda usados
        /// </summary>
        public ParametrosBusquedaAeronaveDto Parametros { get; set; } = new();

        /// <summary>
        /// Resumen de la búsqueda
        /// </summary>
        public ResumenDisponibilidadDto Resumen { get; set; } = new();
    }

    /// <summary>
    /// DTO para una aeronave disponible
    /// </summary>
    public class AeronaveDisponibleDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
        public int TiempoPreparacionMinutos { get; set; }

        /// <summary>
        /// Información del equipo asignado a esta aeronave
        /// </summary>
        public EquipoAsignadoInfoDto? EquipoAsignado { get; set; }

        /// <summary>
        /// Próximo vuelo programado (si existe)
        /// </summary>
        public ProximoVueloDto? ProximoVuelo { get; set; }

        /// <summary>
        /// Cantidad de vuelos programados para el día
        /// </summary>
        public int VuelosDelDia { get; set; }

        /// <summary>
        /// Asientos por clase
        /// </summary>
        public AsientosPorClaseDto? AsientosPorClase { get; set; }
    }

    /// <summary>
    /// DTO para una aeronave no disponible
    /// </summary>
    public class AeronaveNoDisponibleDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }

        /// <summary>
        /// Razón por la que no está disponible
        /// </summary>
        public string Razon { get; set; } = null!;

        /// <summary>
        /// Código de la razón para el frontend
        /// </summary>
        public string CodigoRazon { get; set; } = null!;

        /// <summary>
        /// Vuelo que causa el conflicto (si aplica)
        /// </summary>
        public VueloConflictoDto? VueloConflicto { get; set; }

        /// <summary>
        /// Hora en que estará disponible nuevamente (si aplica)
        /// </summary>
        public string? DisponibleDesde { get; set; }
    }

    /// <summary>
    /// Información del equipo asignado
    /// </summary>
    public class EquipoAsignadoInfoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }

    /// <summary>
    /// Información del próximo vuelo
    /// </summary>
    public class ProximoVueloDto
    {
        public int Id { get; set; }
        public string NumeroVuelo { get; set; } = null!;
        public string HoraSalida { get; set; } = null!;
        public string HoraLlegada { get; set; } = null!;
        public string Ruta { get; set; } = null!; // Ej: "SDQ ? JFK"
    }

    /// <summary>
    /// Información del vuelo que causa conflicto
    /// </summary>
    public class VueloConflictoDto
    {
        public int Id { get; set; }
        public string NumeroVuelo { get; set; } = null!;
        public string HoraSalida { get; set; } = null!;
        public string HoraLlegada { get; set; } = null!;
        public string Ruta { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }

    /// <summary>
    /// Asientos disponibles por clase
    /// </summary>
    public class AsientosPorClaseDto
    {
        public int Primera { get; set; }
        public int Ejecutiva { get; set; }
        public int Economica { get; set; }
        public int Total { get; set; }
    }

    /// <summary>
    /// Parámetros de búsqueda usados
    /// </summary>
    public class ParametrosBusquedaAeronaveDto
    {
        public string Fecha { get; set; } = null!;
        public string HoraSalida { get; set; } = null!;
        public string HoraLlegada { get; set; } = null!;
        public int? VueloIdExcluir { get; set; }
    }

    /// <summary>
    /// Resumen de disponibilidad
    /// </summary>
    public class ResumenDisponibilidadDto
    {
        public int TotalAeronaves { get; set; }
        public int Disponibles { get; set; }
        public int NoDisponibles { get; set; }
        public int EnMantenimiento { get; set; }
        public int SinEquipo { get; set; }
        public int ConConflictoHorario { get; set; }
    }
}