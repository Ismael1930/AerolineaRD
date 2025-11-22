using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace AerolineaRD.Repositories.Implements
{
    public class VueloRepository : GenericRepository<Vuelo>, IVueloRepository
    {
        private readonly AppDbContext _context;

        public VueloRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Vuelo>> BuscarVuelosConFiltrosAsync(string? origen, string? destino, DateTime? fechaSalida, DateTime? fechaRegreso, string? clase, string tipoViaje)
        {
            var query = _context.Vuelos
                .AsNoTracking() // ? Evitar tracking para mejor rendimiento
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Aeronave) // Cargar aeronave
                .ThenInclude(a => a.Asientos) // Cargar SOLO asientos, NO otros vuelos
                .Include(v => v.Reservas) // Cargar reservas para calcular disponibilidad
                .AsQueryable();

            // Filtrar por origen y destino
            if (!string.IsNullOrEmpty(origen))
            {
                query = query.Where(v => v.OrigenCodigo == origen);
            }

            if (!string.IsNullOrEmpty(destino))
            {
                query = query.Where(v => v.DestinoCodigo == destino);
            }

            // Filtrar por tipo de viaje
            if (!string.IsNullOrEmpty(tipoViaje))
            {
                query = query.Where(v => v.TipoVuelo == tipoViaje);
            }

            // Filtrar por fecha de salida
            if (fechaSalida.HasValue)
            {
                var fecha = fechaSalida.Value.Date;
                query = query.Where(v => v.Fecha.Date >= fecha);
            }

            // Traer a memoria
            var vuelosEnMemoria = await query
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            // Ordenar por HoraSalida en memoria
            vuelosEnMemoria = vuelosEnMemoria
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.HoraSalida)
                .ToList();

            // Filtrar por clase en memoria
            if (!string.IsNullOrEmpty(clase))
            {
                var claseNormalizada = NormalizarTexto(clase);
                
                vuelosEnMemoria = vuelosEnMemoria
                    .Where(v => v.Aeronave != null && 
                                v.Aeronave.Asientos != null &&
                                v.Aeronave.Asientos.Any(a => 
                                    NormalizarTexto(a.Clase) == claseNormalizada))
                    .ToList();
            }

            return vuelosEnMemoria;
        }

        public async Task<Vuelo?> ObtenerVueloConDetallesAsync(int id)
        {
            return await _context.Vuelos
                .AsNoTracking() // ? Evitar tracking
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Aeronave)
                .ThenInclude(a => a.Asientos) // Cargar SOLO asientos
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        private static string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            // Convertir a minúsculas y remover acentos
            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in textoNormalizado)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
        }
    }
}
