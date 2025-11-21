using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IPasajeroRepository : IGenericRepository<Pasajero>
    {
        Task<Pasajero?> ObtenerPorPasaporteAsync(string pasaporte);
    }
}