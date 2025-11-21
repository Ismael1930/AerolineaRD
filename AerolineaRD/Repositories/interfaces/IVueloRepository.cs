using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IVueloRepository : IGenericRepository<Vuelo>
    {
        Task<List<Vuelo>> BuscarVuelosConFiltrosAsync(string? origen, string? destino, DateTime? fechaSalida, DateTime? fechaRegreso, string? clase, string tipoViaje);
        Task<Vuelo?> ObtenerVueloConDetallesAsync(int id);
    }
}