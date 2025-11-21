using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class FacturaRepository : GenericRepository<Factura>, IFacturaRepository
    {
        private readonly AppDbContext _context;

        public FacturaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Factura> ObtenerPorCodigoAsync(string codigo)
        {
            return await _context.Facturas
                .Include(f => f.Reserva)
                .FirstOrDefaultAsync(f => f.Codigo == codigo);
        }

        public async Task<Factura> ObtenerPorReservaAsync(string codReserva)
        {
            return await _context.Facturas
                .FirstOrDefaultAsync(f => f.CodReserva == codReserva);
        }
    }
}