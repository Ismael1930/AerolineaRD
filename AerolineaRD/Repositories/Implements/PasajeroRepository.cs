using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class PasajeroRepository : GenericRepository<Pasajero>, IPasajeroRepository
    {
        private readonly AppDbContext _context;

        public PasajeroRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pasajero> ObtenerPorPasaporteAsync(string pasaporte)
        {
            return await _context.Pasajeros
                .FirstOrDefaultAsync(p => p.Pasaporte == pasaporte);
        }
    }
}