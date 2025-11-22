using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class ReservaRepository : GenericRepository<Reserva>, IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Reserva> ObtenerReservaPorCodigoAsync(string codigo)
        {
            return await _context.Reservas
                .Include(r => r.Pasajero)
                .Include(r => r.Vuelo)
                    .ThenInclude(v => v!.Origen)
                .Include(r => r.Vuelo)
                    .ThenInclude(v => v!.Destino)
                .Include(r => r.Factura)
                .FirstOrDefaultAsync(r => r.Codigo == codigo);
        }

        public async Task<List<Reserva>> ObtenerReservasPorClienteAsync(int idCliente)
        {
            return await _context.Reservas
                .Include(r => r.Pasajero)
                .Include(r => r.Vuelo)
                    .ThenInclude(v => v!.Origen)
                .Include(r => r.Vuelo)
                    .ThenInclude(v => v!.Destino)
                .Include(r => r.Factura)
                .Where(r => r.IdCliente == idCliente)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();
        }

        public async Task<bool> ExisteReservaActivaAsync(int idVuelo, string numAsiento)
        {
            return await _context.Reservas
                .AnyAsync(r => r.IdVuelo == idVuelo 
                    && r.NumAsiento == numAsiento 
                    && r.Estado != "Cancelada");
        }
    }
}