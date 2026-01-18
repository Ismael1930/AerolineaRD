using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IRutaRepository : IGenericRepository<Ruta>
    {
        /// <summary>
        /// Obtiene una ruta por origen y destino
        /// </summary>
        Task<Ruta?> ObtenerRutaAsync(string origenCodigo, string destinoCodigo);

        /// <summary>
        /// Obtiene todas las rutas activas
        /// </summary>
        Task<List<Ruta>> ObtenerRutasActivasAsync();

        /// <summary>
        /// Obtiene todas las rutas desde un aeropuerto de origen
        /// </summary>
        Task<List<Ruta>> ObtenerRutasDesdeOrigenAsync(string origenCodigo);

        /// <summary>
        /// Obtiene todas las rutas hacia un aeropuerto de destino
        /// </summary>
        Task<List<Ruta>> ObtenerRutasHaciaDestinoAsync(string destinoCodigo);

        /// <summary>
        /// Verifica si existe una ruta entre dos aeropuertos
        /// </summary>
        Task<bool> ExisteRutaAsync(string origenCodigo, string destinoCodigo);
    }
}
