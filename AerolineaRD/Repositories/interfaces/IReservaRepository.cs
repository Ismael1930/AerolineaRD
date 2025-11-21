using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IReservaRepository : IGenericRepository<Reserva>
    {
        Task<Reserva?> ObtenerReservaPorCodigoAsync(string codigo);
        Task<List<Reserva>> ObtenerReservasPorClienteAsync(int idCliente);
        Task<bool> ExisteReservaActivaAsync(int idVuelo, string numAsiento);
    }
}