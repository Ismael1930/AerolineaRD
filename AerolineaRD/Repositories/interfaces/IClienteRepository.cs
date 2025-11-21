using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Task<Cliente?> ObtenerClientePorUserIdAsync(string userId);
        Task<Cliente?> ObtenerClientePorEmailAsync(string email);
        Task<List<Cliente>> ObtenerClientesConReservasAsync();
    }
}