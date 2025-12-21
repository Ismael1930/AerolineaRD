using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IVueloRepository : IGenericRepository<Vuelo>
    {
        // Updated to accept start/end ranges for departure and return.
        Task<List<Vuelo>> BuscarVuelosConFiltrosAsync(
            string? origen,
            string? destino,
            DateTime? fechaSalidaInicio,
            DateTime? fechaSalidaFin,
            DateTime? fechaRegresoInicio,
            DateTime? fechaRegresoFin,
            string? clase,
            string tipoViaje);

        Task<Vuelo?> ObtenerVueloConDetallesAsync(int id);

        /// <summary>
        /// Verifica si una aeronave está disponible en un horario específico
        /// </summary>
        Task<bool> EstaAeronaveDisponibleAsync(string matricula, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada, int? vueloIdExcluir = null);

        /// <summary>
        /// Verifica la capacidad del aeropuerto en un horario específico (slots disponibles)
        /// </summary>
        Task<bool> AeropuertoTieneCapacidadAsync(string codigoAeropuerto, DateTime fecha, TimeSpan hora, bool esSalida);
        
        /// <summary>
        /// Obtiene el número de asientos disponibles en un vuelo
        /// </summary>
        Task<int> ObtenerAsientosDisponiblesAsync(int idVuelo);
    }
}