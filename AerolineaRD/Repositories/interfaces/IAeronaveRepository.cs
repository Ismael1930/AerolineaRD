using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IAeronaveRepository : IGenericRepository<Aeronave>
    {
        Task<List<Aeronave>> ObtenerAeronavesDisponiblesAsync();
    }
}