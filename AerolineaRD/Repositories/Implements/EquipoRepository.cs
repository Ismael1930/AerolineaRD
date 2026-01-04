using AerolineaRD.Data;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Repositories.Implements
{
    public class EquipoRepository : GenericRepository<Equipo>, IEquipoRepository
    {
        private readonly AppDbContext _context;

        public EquipoRepository(AppDbContext context) : base(context)
   {
  _context = context;
    }

     public async Task<List<Equipo>> ObtenerDisponiblesAsync()
    {
         var ahora = DateTime.Now;

    return await _context.Equipos
   .Include(e => e.EquiposPersonal)
        .ThenInclude(ep => ep.Personal)
        .Where(e => e.Activo && (
     e.Estado == "Disponible" ||
   (e.Estado == "Descanso" && e.DisponibleDesde != null && e.DisponibleDesde <= ahora)
   ))
     .OrderBy(e => e.Nombre)
        .ToListAsync();
   }

  public async Task<Equipo?> ObtenerConMiembrosAsync(int id)
      {
     return await _context.Equipos
  .Include(e => e.EquiposPersonal.Where(ep => ep.Activo))
     .ThenInclude(ep => ep.Personal)
     .FirstOrDefaultAsync(e => e.Id == id);
     }

 public async Task<Equipo?> ObtenerConMiembrosYAsignacionAsync(int id)
{
  return await _context.Equipos
       .Include(e => e.EquiposPersonal.Where(ep => ep.Activo))
 .ThenInclude(ep => ep.Personal)
   .Include(e => e.AsignacionesAeronave.Where(a => a.Activa))
         .ThenInclude(a => a.Aeronave)
          .FirstOrDefaultAsync(e => e.Id == id);
     }

        public async Task<bool> CodigoExisteAsync(string codigo, int? idExcluir = null)
   {
      var query = _context.Equipos.Where(e => e.Codigo == codigo);
 
            if (idExcluir.HasValue)
      query = query.Where(e => e.Id != idExcluir.Value);

  return await query.AnyAsync();
  }

        public async Task<AsignacionEquipoAeronave?> ObtenerAsignacionActivaPorAeronaveAsync(string matricula)
  {
 return await _context.AsignacionesEquipoAeronave
      .Include(a => a.Equipo)
           .ThenInclude(e => e.EquiposPersonal.Where(ep => ep.Activo))
   .ThenInclude(ep => ep.Personal)
       .Include(a => a.Aeronave)
     .FirstOrDefaultAsync(a => a.Matricula == matricula && a.Activa);
        }

   public async Task<AsignacionEquipoAeronave?> ObtenerAsignacionActivaPorEquipoAsync(int idEquipo)
        {
       return await _context.AsignacionesEquipoAeronave
        .Include(a => a.Equipo)
  .Include(a => a.Aeronave)
        .FirstOrDefaultAsync(a => a.IdEquipo == idEquipo && a.Activa);
    }

public async Task<List<AsignacionEquipoAeronave>> ObtenerTodasAsignacionesAsync()
 {
  return await _context.AsignacionesEquipoAeronave
        .Include(a => a.Equipo)
  .ThenInclude(e => e.EquiposPersonal.Where(ep => ep.Activo))
 .ThenInclude(ep => ep.Personal)
     .Include(a => a.Aeronave)
        .OrderByDescending(a => a.Activa)
     .ThenByDescending(a => a.FechaAsignacion)
  .ToListAsync();
  }

        public async Task AsignarMiembrosAsync(int idEquipo, List<int> idsPersonal)
   {
    var miembros = idsPersonal.Select(idPersonal => new EquipoPersonal
  {
IdEquipo = idEquipo,
     IdPersonal = idPersonal,
    FechaAsignacion = DateTime.Now,
         Activo = true
 }).ToList();

            await _context.EquipoPersonal.AddRangeAsync(miembros);
        }

        public async Task DesasignarTodosMiembrosAsync(int idEquipo)
   {
            var miembros = await _context.EquipoPersonal
        .Where(ep => ep.IdEquipo == idEquipo && ep.Activo)
       .ToListAsync();

   foreach (var miembro in miembros)
            {
         miembro.Activo = false;
       }
 }
    }
}
