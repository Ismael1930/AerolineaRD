using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class EquipajeRepository : GenericRepository<Equipaje>, IEquipajeRepository
    {
        private readonly AppDbContext _context;

        public EquipajeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Equipaje>> ObtenerEquipajesPorPasajeroAsync(int idPasajero)
        {
            return await _context.Equipajes
                .Include(e => e.Pasajero)
                .Where(e => e.IdPasajero == idPasajero)
                .ToListAsync();
        }
    }
}