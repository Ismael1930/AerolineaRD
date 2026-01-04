using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IPersonalRepository : IGenericRepository<Personal>
    {
        Task<List<Personal>> ObtenerPorRolAsync(string rol);
        Task<List<Personal>> ObtenerDisponiblesAsync();
Task<Personal?> ObtenerConEquiposAsync(int id);
    Task<bool> TieneCertificacionParaModeloAsync(int idPersonal, string modelo);
        Task<bool> EstaDisponibleAsync(int idPersonal, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada);
        Task<List<Personal>> ObtenerPorIdsAsync(List<int> ids);
    }
}
