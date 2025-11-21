using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface ITripulacionRepository : IGenericRepository<Tripulacion>
    {
        Task<List<Tripulacion>> ObtenerPorRolAsync(string rol);
    }
}