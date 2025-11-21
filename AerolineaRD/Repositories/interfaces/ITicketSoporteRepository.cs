using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface ITicketSoporteRepository : IGenericRepository<TicketSoporte>
    {
        Task<List<TicketSoporte>> ObtenerPorClienteAsync(int idCliente);
        Task<List<TicketSoporte>> ObtenerTicketsAbiertosiAsync();
    }
}