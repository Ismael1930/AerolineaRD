using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IFacturaRepository : IGenericRepository<Factura>
    {
        Task<Factura?> ObtenerPorCodigoAsync(string codigo);
        Task<Factura?> ObtenerPorReservaAsync(string codReserva);
    }
}