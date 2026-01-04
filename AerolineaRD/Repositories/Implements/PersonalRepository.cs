using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class PersonalRepository : GenericRepository<Personal>, IPersonalRepository
    {
        private readonly AppDbContext _context;

   public PersonalRepository(AppDbContext context) : base(context)
    {
            _context = context;
  }

        public async Task<List<Personal>> ObtenerPorRolAsync(string rol)
        {
     return await _context.Personal
      .Where(p => p.Rol == rol && p.Activo)
 .OrderBy(p => p.Apellido)
       .ThenBy(p => p.Nombre)
      .ToListAsync();
     }

 public async Task<List<Personal>> ObtenerDisponiblesAsync()
        {
            var ahora = DateTime.Now;
            
            // ? Traer datos a memoria y filtrar en C# (EF.Functions.DateDiffMinute no existe en SQLite)
        var personal = await _context.Personal
                .Where(p => p.Activo)
   .ToListAsync();

 // Filtrar en memoria
       var disponibles = personal
  .Where(p => 
            p.Estado == "Disponible" ||
 (p.Estado == "Descanso" && p.UltimoVueloFin != null &&
   (ahora - p.UltimoVueloFin.Value).TotalMinutes >= p.TiempoDescansoMinutos)
      )
        .OrderBy(p => p.Rol)
       .ThenBy(p => p.Apellido)
       .ToList();

  return disponibles;
 }

        public async Task<Personal?> ObtenerConEquiposAsync(int id)
        {
      return await _context.Personal
.Include(p => p.EquiposPersonal)
   .ThenInclude(ep => ep.Equipo)
           .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> TieneCertificacionParaModeloAsync(int idPersonal, string modelo)
        {
            var personal = await _context.Personal.FindAsync(idPersonal);
   
            if (personal == null || string.IsNullOrEmpty(personal.CertificacionesAeronave))
    return false;

            var certificaciones = personal.CertificacionesAeronave
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(c => c.Trim())
 .ToList();

   return certificaciones.Any(c => c.Equals(modelo, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> EstaDisponibleAsync(int idPersonal, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada)
        {
        var personal = await _context.Personal.FindAsync(idPersonal);
         
      if (personal == null || !personal.Activo)
                return false;

            // Verificar si está en descanso y si ya cumplió el tiempo
        if (personal.Estado == "Descanso" && personal.UltimoVueloFin.HasValue)
    {
   var minutosDescanso = (DateTime.Now - personal.UltimoVueloFin.Value).TotalMinutes;
   if (minutosDescanso < personal.TiempoDescansoMinutos)
    return false;
 }

         // Verificar si está en servicio
   if (personal.Estado == "En Servicio")
  return false;

         // Buscar equipos activos donde esté este personal
    var equiposActivos = await _context.EquipoPersonal
          .Where(ep => ep.IdPersonal == idPersonal && ep.Activo)
                .Select(ep => ep.IdEquipo)
        .ToListAsync();

            if (!equiposActivos.Any())
  return true;

            // Verificar si algún equipo tiene asignación activa
            var tieneAsignacionActiva = await _context.AsignacionesEquipoAeronave
      .AnyAsync(a => equiposActivos.Contains(a.IdEquipo) && a.Activa);

  return !tieneAsignacionActiva;
        }

    public async Task<List<Personal>> ObtenerPorIdsAsync(List<int> ids)
  {
            return await _context.Personal
                .Where(p => ids.Contains(p.Id) && p.Activo)
    .ToListAsync();
        }
    }
}
