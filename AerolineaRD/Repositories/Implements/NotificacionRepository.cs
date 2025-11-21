using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class NotificacionRepository : GenericRepository<Notificacion>, INotificacionRepository
    {
        private readonly AppDbContext _context;

        public NotificacionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Notificacion>> ObtenerPorClienteAsync(int idCliente)
        {
            return await _context.Notificaciones
                .Where(n => n.IdCliente == idCliente)
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }

        public async Task<List<Notificacion>> ObtenerNoLeidasAsync(int idCliente)
        {
            return await _context.Notificaciones
                .Where(n => n.IdCliente == idCliente && !n.Leida)
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }
    }
}