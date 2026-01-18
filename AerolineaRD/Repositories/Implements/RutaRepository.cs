using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class RutaRepository : GenericRepository<Ruta>, IRutaRepository
    {
        public RutaRepository(AppDbContext context) : base(context) { }

        public async Task<Ruta?> ObtenerRutaAsync(string origenCodigo, string destinoCodigo)
        {
            return await Context.Rutas
                .Include(r => r.Origen)
                .Include(r => r.Destino)
                .FirstOrDefaultAsync(r => 
                    r.OrigenCodigo == origenCodigo && 
                    r.DestinoCodigo == destinoCodigo &&
                    r.Activa);
        }

        public async Task<List<Ruta>> ObtenerRutasActivasAsync()
        {
            return await Context.Rutas
                .Include(r => r.Origen)
                .Include(r => r.Destino)
                .Where(r => r.Activa)
                .OrderBy(r => r.OrigenCodigo)
                .ThenBy(r => r.DestinoCodigo)
                .ToListAsync();
        }

        public async Task<List<Ruta>> ObtenerRutasDesdeOrigenAsync(string origenCodigo)
        {
            return await Context.Rutas
                .Include(r => r.Origen)
                .Include(r => r.Destino)
                .Where(r => r.OrigenCodigo == origenCodigo && r.Activa)
                .OrderBy(r => r.DestinoCodigo)
                .ToListAsync();
        }

        public async Task<List<Ruta>> ObtenerRutasHaciaDestinoAsync(string destinoCodigo)
        {
            return await Context.Rutas
                .Include(r => r.Origen)
                .Include(r => r.Destino)
                .Where(r => r.DestinoCodigo == destinoCodigo && r.Activa)
                .OrderBy(r => r.OrigenCodigo)
                .ToListAsync();
        }

        public async Task<bool> ExisteRutaAsync(string origenCodigo, string destinoCodigo)
        {
            return await Context.Rutas
                .AnyAsync(r => 
                    r.OrigenCodigo == origenCodigo && 
                    r.DestinoCodigo == destinoCodigo);
        }
    }
}
