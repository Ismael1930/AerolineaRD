using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IEquipajeRepository : IGenericRepository<Equipaje>
    {
        Task<List<Equipaje>> ObtenerEquipajesPorPasajeroAsync(int idPasajero);
    }
}