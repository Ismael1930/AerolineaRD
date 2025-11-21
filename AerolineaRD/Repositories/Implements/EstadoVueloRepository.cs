using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class EstadoVueloRepository : GenericRepository<EstadoVuelo>, IEstadoVueloRepository
    {
        private readonly AppDbContext _context;

        public EstadoVueloRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EstadoVuelo> ObtenerPorVueloAsync(int idVuelo)
        {
            return await _context.EstadosVuelo
                .Include(e => e.Vuelo)
                .FirstOrDefaultAsync(e => e.IdVuelo == idVuelo);
        }
    }
}