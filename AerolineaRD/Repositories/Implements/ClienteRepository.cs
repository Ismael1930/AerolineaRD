using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObtenerClientePorUserIdAsync(string userId)
        {
            return await _context.Clientes
                .Include(c => c.User)
                .Include(c => c.Reservas)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cliente?> ObtenerClientePorEmailAsync(string email)
        {
            return await _context.Clientes
                .Include(c => c.User)
                .Include(c => c.Reservas)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<List<Cliente>> ObtenerClientesConReservasAsync()
        {
            return await _context.Clientes
                .Include(c => c.User)
                .Include(c => c.Reservas)
                .ToListAsync();
        }
    }
}