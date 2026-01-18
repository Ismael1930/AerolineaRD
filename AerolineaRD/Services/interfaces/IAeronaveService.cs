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

        /// <summary>
        /// Obtiene las aeronaves disponibles para un horario específico
        /// Filtra por: operativas, con equipo asignado, y sin conflictos de horario
        /// </summary>
        /// <param name="fecha">Fecha del vuelo</param>
        /// <param name="horaSalida">Hora de salida del vuelo</param>
        /// <param name="horaLlegada">Hora de llegada del vuelo</param>
        /// <param name="vueloIdExcluir">ID del vuelo a excluir (para edición)</param>
        Task<AeronavesDisponiblesResponseDto> ObtenerAeronavesDisponiblesParaHorarioAsync(
            DateTime fecha, 
            TimeSpan horaSalida, 
            TimeSpan horaLlegada,
            int? vueloIdExcluir = null);
    }
}