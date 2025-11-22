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

        // CAMBIO: Ahora busca asientos por matrícula de aeronave
        public async Task<List<Asiento>> ObtenerAsientosPorVueloAsync(int idVuelo)
        {
            // Primero obtener la matrícula del vuelo
            var vuelo = await _context.Vuelos
                .Where(v => v.Id == idVuelo)
                .Select(v => v.Matricula)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(vuelo))
                return new List<Asiento>();

            // Luego obtener los asientos de esa aeronave
            return await _context.Asientos
                .Where(a => a.Matricula == vuelo)
                .OrderBy(a => a.NumeroAsiento)
                .ToListAsync();
        }

        public async Task<Asiento?> ObtenerAsientoPorNumeroAsync(string numero)
        {
            return await _context.Asientos
                .FirstOrDefaultAsync(a => a.Numero == numero);
        }
    }
}