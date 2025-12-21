using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class TripulacionRepository : GenericRepository<Tripulacion>, ITripulacionRepository
    {
        private readonly AppDbContext _context;

        public TripulacionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Tripulacion>> ObtenerPorRolAsync(string rol)
        {
            return await _context.Tripulaciones
                .Where(t => t.Rol == rol)
                .ToListAsync();
        }

        public async Task<bool> EstaTripulacionDisponibleAsync(int idTripulacion, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada, int? vueloIdExcluir = null)
        {
            // Obtener información del tripulante
            var tripulante = await _context.Tripulaciones
                .Include(t => t.Vuelos)
                .FirstOrDefaultAsync(t => t.Id == idTripulacion);

            if (tripulante == null)
                return false;

            // Tiempo de descanso en minutos (por defecto 480 = 8 horas)
            int tiempoDescansoMinutos = tripulante.TiempoDescansoMinutos > 0
                ? tripulante.TiempoDescansoMinutos
                : 480;

            // Buscar vuelos asignados al tripulante en la misma fecha
            var vuelosAsignados = await _context.VueloTripulaciones
                .Where(vt => vt.IdTripulacion == idTripulacion
                      && (!vueloIdExcluir.HasValue || vt.IdVuelo != vueloIdExcluir.Value))
                .Include(vt => vt.Vuelo)
                        .Select(vt => vt.Vuelo)
                      .Where(v => v != null && v.Fecha.Date == fecha.Date)
                .ToListAsync();

            if (!vuelosAsignados.Any())
                return true;

            // Verificar solapamiento incluyendo tiempo de descanso
                foreach (var vuelo in vuelosAsignados)
     {
     if (vuelo == null) continue;

      // Calcular tiempo de descanso necesario
              var horaLlegadaConDescanso = vuelo.HoraLlegada.Add(TimeSpan.FromMinutes(tiempoDescansoMinutos));
  var horaSalidaConDescanso = vuelo.HoraSalida.Subtract(TimeSpan.FromMinutes(tiempoDescansoMinutos));

 // Verificar conflictos
         bool conflictoSalida = horaSalida >= vuelo.HoraSalida && horaSalida < horaLlegadaConDescanso;
 bool conflictoLlegada = horaLlegada > horaSalidaConDescanso && horaLlegada <= vuelo.HoraLlegada;
     bool conflictoCompleto = horaSalida <= horaSalidaConDescanso && horaLlegada >= horaLlegadaConDescanso;

    if (conflictoSalida || conflictoLlegada || conflictoCompleto)
         return false;
   }

   return true;
 }

    public async Task<bool> TieneCertificacionParaAeronaveAsync(int idTripulacion, string modeloAeronave)
 {
        var tripulante = await _context.Tripulaciones
       .FirstOrDefaultAsync(t => t.Id == idTripulacion);

        if (tripulante == null || string.IsNullOrEmpty(tripulante.CertificacionesAeronave))
  return false;

        // Las certificaciones están separadas por coma
        var certificaciones = tripulante.CertificacionesAeronave
  .Split(',', StringSplitOptions.RemoveEmptyEntries)
 .Select(c => c.Trim().ToLowerInvariant())
          .ToList();

        // Verificar si el modelo está certificado
        return certificaciones.Any(c => modeloAeronave.ToLowerInvariant().Contains(c));
  }
    }
}