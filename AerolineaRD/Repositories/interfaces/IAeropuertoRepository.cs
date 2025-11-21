using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IAeropuertoRepository : IGenericRepository<Aeropuerto>
    {
        Task<List<Aeropuerto>> ObtenerTodosOrdenadosAsync();
    }
}