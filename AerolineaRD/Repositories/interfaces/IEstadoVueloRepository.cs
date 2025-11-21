using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IEstadoVueloRepository : IGenericRepository<EstadoVuelo>
    {
        Task<EstadoVuelo?> ObtenerPorVueloAsync(int idVuelo);
    }
}