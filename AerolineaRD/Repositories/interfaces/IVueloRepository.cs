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
    }
}