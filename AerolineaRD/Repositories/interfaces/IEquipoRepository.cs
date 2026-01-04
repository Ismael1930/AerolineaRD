using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface IEquipoRepository : IGenericRepository<Equipo>
    {
        Task<List<Equipo>> ObtenerDisponiblesAsync();
  Task<Equipo?> ObtenerConMiembrosAsync(int id);
        Task<Equipo?> ObtenerConMiembrosYAsignacionAsync(int id);
    Task<bool> CodigoExisteAsync(string codigo, int? idExcluir = null);
        Task<AsignacionEquipoAeronave?> ObtenerAsignacionActivaPorAeronaveAsync(string matricula);
        Task<AsignacionEquipoAeronave?> ObtenerAsignacionActivaPorEquipoAsync(int idEquipo);
        Task<List<AsignacionEquipoAeronave>> ObtenerTodasAsignacionesAsync();
      Task AsignarMiembrosAsync(int idEquipo, List<int> idsPersonal);
        Task DesasignarTodosMiembrosAsync(int idEquipo);
    }
}
