using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IAeronaveService
    {
        Task<AeronaveResponseDto> CrearAeronaveAsync(CrearAeronaveDto dto);
        Task<List<AeronaveResponseDto>> ObtenerAeronavesDisponiblesAsync();
        Task<List<AeronaveResponseDto>> ObtenerTodasAsync();
        Task<AeronaveResponseDto?> ObtenerPorMatriculaAsync(string matricula);
        Task<AeronaveResponseDto> ActualizarAeronaveAsync(ActualizarAeronaveDto dto);
        Task<bool> EliminarAeronaveAsync(string matricula);
        
        /// <summary>
        /// Obtiene todas las aeronaves con información de disponibilidad de asientos
        /// </summary>
        Task<List<AeronaveConDisponibilidadDto>> ObtenerTodasConDisponibilidadAsync();
   
        /// <summary>
        /// Obtiene una aeronave específica con información de disponibilidad de asientos
        /// </summary>
        Task<AeronaveConDisponibilidadDto?> ObtenerConDisponibilidadAsync(string matricula);
    }
}