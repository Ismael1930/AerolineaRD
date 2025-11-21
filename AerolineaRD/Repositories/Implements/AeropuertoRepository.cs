using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class AeropuertoRepository : GenericRepository<Aeropuerto>, IAeropuertoRepository
    {
        private readonly AppDbContext _context;

        public AeropuertoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Aeropuerto>> ObtenerTodosOrdenadosAsync()
        {
            return await _context.Aeropuertos
                .OrderBy(a => a.Ciudad)
                .ToListAsync();
        }
    }
}