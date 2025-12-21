using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IReservaRepository : IGenericRepository<Reserva>
    {
        Task<Reserva?> ObtenerReservaPorCodigoAsync(string codigo);
        Task<List<Reserva>> ObtenerReservasPorClienteAsync(int idCliente);
        Task<bool> ExisteReservaActivaAsync(int idVuelo, string numAsiento);
        Task<List<Reserva>> ObtenerTodasConDetallesAsync();
        
        /// <summary>
        /// Obtiene una reserva con todos sus detalles (vuelo, cliente, pasajero)
        /// </summary>
        Task<Reserva?> ObtenerReservaConDetallesAsync(string codigo);
     
        /// <summary>
      /// Obtiene una reserva por vuelo y número de asiento
    /// </summary>
        Task<Reserva?> ObtenerPorVueloYAsientoAsync(int idVuelo, string? numAsiento);
    }
}