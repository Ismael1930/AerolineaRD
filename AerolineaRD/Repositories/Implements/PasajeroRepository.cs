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

        public async Task<Pasajero?> ObtenerPorUserIdAsync(string userId)
        {
            // Intentar buscar pasajero cuyo cliente asociado tenga UserId = userId
            var pasajero = await _context.Pasajeros
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Cliente != null && p.Cliente.UserId == userId);

            if (pasajero != null)
                return pasajero;

            // Fallback: si frontend envía el pasaporte en lugar del userId, buscar por pasaporte
            return await _context.Pasajeros
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Pasaporte == userId);
        }
    }
}