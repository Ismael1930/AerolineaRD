using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface INotificacionRepository : IGenericRepository<Notificacion>
    {
        Task<List<Notificacion>> ObtenerPorClienteAsync(int idCliente);
        Task<List<Notificacion>> ObtenerNoLeidasAsync(int idCliente);
    }
}