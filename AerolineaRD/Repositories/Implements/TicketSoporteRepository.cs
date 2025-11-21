using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class TicketSoporteRepository : GenericRepository<TicketSoporte>, ITicketSoporteRepository
    {
        private readonly AppDbContext _context;

        public TicketSoporteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TicketSoporte>> ObtenerPorClienteAsync(int idCliente)
        {
            return await _context.TicketsSoporte
                .Include(t => t.Cliente)
                .Where(t => t.IdCliente == idCliente)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<TicketSoporte>> ObtenerTicketsAbiertosiAsync()
        {
            return await _context.TicketsSoporte
                .Include(t => t.Cliente)
                .Where(t => t.Estado == "Abierto" || t.Estado == "En Proceso")
                .OrderBy(t => t.Prioridad == "Alta" ? 1 : t.Prioridad == "Media" ? 2 : 3)
                .ThenBy(t => t.FechaCreacion)
                .ToListAsync();
        }
    }
}