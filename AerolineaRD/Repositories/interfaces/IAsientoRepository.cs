using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IAsientoRepository : IGenericRepository<Asiento>
    {
        Task<List<Asiento>> ObtenerAsientosPorVueloAsync(int idVuelo);
        Task<Asiento?> ObtenerAsientoPorNumeroAsync(string numero);
    }
}