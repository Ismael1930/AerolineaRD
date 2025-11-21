using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class TripulacionRepository : GenericRepository<Tripulacion>, ITripulacionRepository
    {
        private readonly AppDbContext _context;

        public TripulacionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Tripulacion>> ObtenerPorRolAsync(string rol)
        {
            return await _context.Tripulaciones
                .Where(t => t.Rol == rol)
                .ToListAsync();
        }
    }
}