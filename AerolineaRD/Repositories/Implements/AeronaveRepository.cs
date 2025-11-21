using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class AeronaveRepository : GenericRepository<Aeronave>, IAeronaveRepository
    {
        private readonly AppDbContext _context;

        public AeronaveRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Aeronave>> ObtenerAeronavesDisponiblesAsync()
        {
            return await _context.Aeronaves
                .Where(a => a.Estado == "Operativa")
                .ToListAsync();
        }
    }
}