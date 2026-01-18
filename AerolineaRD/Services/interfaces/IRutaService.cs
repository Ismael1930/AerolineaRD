using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IRutaService
    {
        /// <summary>
        /// Obtiene la duración de vuelo entre dos aeropuertos (sin cálculos adicionales)
        /// </summary>
        Task<RutaDuracionDto> ObtenerDuracionRutaAsync(string origenCodigo, string destinoCodigo);

        /// <summary>
        /// Obtiene la duración, hora de llegada calculada y precio sugerido
        /// </summary>
        /// <param name="origenCodigo">Código del aeropuerto de origen</param>
        /// <param name="destinoCodigo">Código del aeropuerto de destino</param>
        /// <param name="horaSalida">Hora de salida para calcular hora de llegada (opcional)</param>
        Task<RutaDuracionDto> ObtenerInfoRutaCompletaAsync(string origenCodigo, string destinoCodigo, TimeSpan? horaSalida = null);

        /// <summary>
        /// Obtiene todas las rutas activas
        /// </summary>
        Task<List<RutaDto>> ObtenerTodasLasRutasAsync();

        /// <summary>
        /// Obtiene las rutas disponibles desde un aeropuerto
        /// </summary>
        Task<List<RutaDto>> ObtenerRutasDesdeOrigenAsync(string origenCodigo);

        /// <summary>
        /// Obtiene las rutas disponibles hacia un aeropuerto
        /// </summary>
        Task<List<RutaDto>> ObtenerRutasHaciaDestinoAsync(string destinoCodigo);

        /// <summary>
        /// Crea una nueva ruta
        /// </summary>
        Task<RutaDto> CrearRutaAsync(CrearRutaDto dto);

        /// <summary>
        /// Actualiza una ruta existente
        /// </summary>
        Task<RutaDto?> ActualizarRutaAsync(ActualizarRutaDto dto);

        /// <summary>
        /// Elimina (desactiva) una ruta
        /// </summary>
        Task<bool> EliminarRutaAsync(int id);
    }
}
