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

        public async Task<List<Vuelo>> BuscarVuelosConFiltrosAsync(string? origen, string? destino, DateTime? fechaSalidaInicio, DateTime? fechaSalidaFin, DateTime? fechaRegresoInicio, DateTime? fechaRegresoFin, string? clase, string tipoViaje)
        {
            var query = _context.Vuelos
                .AsNoTracking()
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Aeronave)
                    .ThenInclude(a => a.Asientos)
                .Include(v => v.Reservas)
                .AsQueryable();

            // Filtrar por origen y destino (igual que antes)
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

            // Filtrar por rango de fecha de salida (inclusive)
            if (fechaSalidaInicio.HasValue && fechaSalidaFin.HasValue)
            {
                var start = fechaSalidaInicio.Value.Date;
                var end = fechaSalidaFin.Value.Date;
                query = query.Where(v => v.Fecha.Date >= start && v.Fecha.Date <= end);
            }
            else if (fechaSalidaInicio.HasValue)
            {
                var start = fechaSalidaInicio.Value.Date;
                query = query.Where(v => v.Fecha.Date >= start);
            }
            else if (fechaSalidaFin.HasValue)
            {
                var end = fechaSalidaFin.Value.Date;
                query = query.Where(v => v.Fecha.Date <= end);
            }


            // Traer a memoria y ordenar
            var vuelosEnMemoria = await query
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            vuelosEnMemoria = vuelosEnMemoria
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.HoraSalida)
                .ToList();

            // Filtrar por clase en memoria (igual que antes)
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
                .AsNoTracking()
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Aeronave)
                    .ThenInclude(a => a.Asientos)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        private static string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

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
