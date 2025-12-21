using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IAeropuertoService
    {
        Task<List<AeropuertoDto>> ObtenerTodosAsync();
        
        /// <summary>
        /// Obtiene el reporte de capacidad de un aeropuerto específico en una fecha
        /// </summary>
        Task<AeropuertoCapacidadDto> ObtenerCapacidadAeropuertoAsync(string codigoAeropuerto, DateTime fecha);

        /// <summary>
        /// Obtiene el reporte de capacidad de todos los aeropuertos en una fecha
        /// </summary>
        Task<ReporteCapacidadAeropuertosDto> ObtenerReporteCapacidadTodosAsync(DateTime fecha);
    }
}