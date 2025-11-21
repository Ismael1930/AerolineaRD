using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class AsientoRepository : GenericRepository<Asiento>, IAsientoRepository
    {
        private readonly AppDbContext _context;

        public AsientoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Asiento>> ObtenerAsientosPorVueloAsync(int idVuelo)
        {
            return await _context.Asientos
                .Where(a => a.IdVuelo == idVuelo)
                .OrderBy(a => a.Numero)
                .ToListAsync();
        }

        public async Task<Asiento> ObtenerAsientoPorNumeroAsync(string numero)
        {
            return await _context.Asientos
                .FirstOrDefaultAsync(a => a.Numero == numero);
        }
    }
}