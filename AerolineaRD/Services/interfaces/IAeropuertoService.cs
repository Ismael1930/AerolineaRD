using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IAeropuertoService
    {
        Task<List<AeropuertoDto>> ObtenerTodosAsync();
      
        /// <summary>
 /// Obtiene el reporte de capacidad de un aeropuerto específico en un rango de fechas
    /// Muestra solo los días con vuelos programados en formato calendario
        /// </summary>
    Task<AeropuertoCapacidadDto> ObtenerCapacidadAeropuertoAsync(
       string codigoAeropuerto, 
       DateTime fechaInicio, 
            DateTime fechaFin);

      /// <summary>
        /// Obtiene el reporte de capacidad de todos los aeropuertos en un rango de fechas
  /// </summary>
  Task<ReporteCapacidadAeropuertosDto> ObtenerReporteCapacidadTodosAsync(
    DateTime fechaInicio, 
        DateTime fechaFin);
  }
}