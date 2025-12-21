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

            // Traer a memoria primero
            var vuelosEnMemoria = await query
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            // ? Filtrar por clase EN MEMORIA (después de traer los datos)
            if (!string.IsNullOrEmpty(clase))
            {
                // Normalizar y extraer solo la primera palabra
                // "Primera Clase" -> "primera" -> coincide con "Primera"
                var claseNormalizada = NormalizarClase(clase);
                vuelosEnMemoria = vuelosEnMemoria
                    .Where(v => v.Clase != null && NormalizarClase(v.Clase) == claseNormalizada)
                    .ToList();
            }

            // Ordenar por fecha y hora
            vuelosEnMemoria = vuelosEnMemoria
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.HoraSalida)
                .ToList();

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

        public async Task<bool> EstaAeronaveDisponibleAsync(string matricula, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada, int? vueloIdExcluir = null)
        {
            if (string.IsNullOrEmpty(matricula))
                return false;

            // Obtener información de la aeronave
            var aeronave = await _context.Aeronaves
                .FirstOrDefaultAsync(a => a.Matricula == matricula);

            if (aeronave == null)
                return false;

            // ? Verificar que la aeronave esté operativa
            if (aeronave.Estado != "Operativa")
                return false;

            int tiempoPreparacionMinutos = aeronave.TiempoPreparacionMinutos > 0 
                ? aeronave.TiempoPreparacionMinutos 
                : 120;

            // Buscar vuelos con la misma aeronave en la misma fecha
            var vuelosConflictivos = await _context.Vuelos
                .Where(v => v.Matricula == matricula 
                            && v.Fecha.Date == fecha.Date
                            && (!vueloIdExcluir.HasValue || v.Id != vueloIdExcluir.Value)
                            && v.Estado != "Cancelado") // ? Ignorar vuelos cancelados
                .ToListAsync();

            if (!vuelosConflictivos.Any())
                return true;

            // Verificar solapamiento incluyendo tiempo de preparación
            foreach (var vuelo in vuelosConflictivos)
            {
                var horaLlegadaConPreparacion = vuelo.HoraLlegada.Add(TimeSpan.FromMinutes(tiempoPreparacionMinutos));
                var horaSalidaConPreparacion = vuelo.HoraSalida.Subtract(TimeSpan.FromMinutes(tiempoPreparacionMinutos));

                bool conflictoSalida = horaSalida >= vuelo.HoraSalida && horaSalida < horaLlegadaConPreparacion;
                bool conflictoLlegada = horaLlegada > horaSalidaConPreparacion && horaLlegada <= vuelo.HoraLlegada;
                bool conflictoCompleto = horaSalida <= horaSalidaConPreparacion && horaLlegada >= horaLlegadaConPreparacion;

                if (conflictoSalida || conflictoLlegada || conflictoCompleto)
                    return false;
            }

            return true;
        }

        public async Task<bool> AeropuertoTieneCapacidadAsync(string codigoAeropuerto, DateTime fecha, TimeSpan hora, bool esSalida)
        {
            // Obtener información del aeropuerto
            var aeropuerto = await _context.Aeropuertos
                .FirstOrDefaultAsync(a => a.Codigo == codigoAeropuerto);

            if (aeropuerto == null)
                return false;

            int capacidadPorHora = aeropuerto.CapacidadVuelosPorHora > 0 
                ? aeropuerto.CapacidadVuelosPorHora 
                : 10;

            // Calcular el rango de la hora (ej: 10:00 a 10:59)
            var horaInicio = new TimeSpan(hora.Hours, 0, 0);
            var horaFin = horaInicio.Add(TimeSpan.FromHours(1));

            // Contar vuelos en esa franja horaria
            var vuelosEnHora = await _context.Vuelos
                .Where(v => v.Fecha.Date == fecha.Date
                            && v.Estado != "Cancelado")
                .Where(v => esSalida 
                    ? (v.OrigenCodigo == codigoAeropuerto && v.HoraSalida >= horaInicio && v.HoraSalida < horaFin)
                    : (v.DestinoCodigo == codigoAeropuerto && v.HoraLlegada >= horaInicio && v.HoraLlegada < horaFin))
                .CountAsync();

            return vuelosEnHora < capacidadPorHora;
        }

        public async Task<int> ObtenerAsientosDisponiblesAsync(int idVuelo)
        {
            var vuelo = await _context.Vuelos
                .Include(v => v.Aeronave)
                .ThenInclude(a => a != null ? a.Asientos : null)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Id == idVuelo);

            if (vuelo?.Aeronave?.Asientos == null)
                return 0;

            // Total de asientos de la aeronave
            int totalAsientos = vuelo.Aeronave.Asientos.Count;

            // Asientos reservados (confirmados)
            int asientosReservados = vuelo.Reservas
                .Count(r => r.Estado == "Confirmada");

            return totalAsientos - asientosReservados;
        }

        /// <summary>
        /// Normaliza el nombre de la clase para comparación
        /// "Primera Clase" -> "primera"
        /// "Primera" -> "primera"
        /// "Ejecutiva" -> "ejecutiva"
        /// "Economica" -> "economica"
        /// </summary>
        private static string NormalizarClase(string? clase)
        {
            if (string.IsNullOrEmpty(clase))
                return string.Empty;

            // Tomar solo la primera palabra y normalizar
            var primeraPalabra = clase.Split(' ')[0];
            return NormalizarTexto(primeraPalabra);
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
