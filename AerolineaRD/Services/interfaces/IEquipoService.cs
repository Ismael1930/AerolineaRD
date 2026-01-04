using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IEquipoService
{
        // ========== PERSONAL ==========
        Task<List<PersonalDto>> ObtenerTodoPersonalAsync();
        Task<List<PersonalDto>> ObtenerPersonalPorRolAsync(string rol);
        Task<List<PersonalDto>> ObtenerPersonalDisponibleAsync();
        Task<PersonalDto?> ObtenerPersonalPorIdAsync(int id);
        Task<OperationResult<PersonalDto>> CrearPersonalAsync(CrearPersonalDto dto);
    Task<OperationResult<PersonalDto>> ActualizarPersonalAsync(ActualizarPersonalDto dto);
 Task<bool> EliminarPersonalAsync(int id);

   // ========== EQUIPOS ==========
        Task<List<EquipoDto>> ObtenerTodosEquiposAsync();
 Task<List<EquipoDto>> ObtenerEquiposDisponiblesAsync();
     Task<EquipoDetalleDto?> ObtenerEquipoPorIdAsync(int id);
        Task<OperationResult<EquipoDetalleDto>> CrearEquipoAsync(CrearEquipoDto dto);
 Task<OperationResult<EquipoDetalleDto>> ActualizarEquipoAsync(ActualizarEquipoDto dto);
Task<bool> EliminarEquipoAsync(int id);
     Task<ValidacionEquipoDto> ValidarComposicionEquipoAsync(List<int> idsPersonal);

        // ========== ASIGNACIONES ==========
        Task<List<AsignacionAeronaveDto>> ObtenerTodasAsignacionesAsync();
  Task<AsignacionAeronaveDto?> ObtenerAsignacionActivaPorAeronaveAsync(string matricula);
        Task<AsignacionAeronaveDto?> ObtenerAsignacionActivaPorEquipoAsync(int idEquipo);
        Task<OperationResult<AsignacionAeronaveDto>> AsignarEquipoAeronaveAsync(AsignarEquipoAeronaveDto dto);
     Task<OperationResult<AsignacionAeronaveDto>> DesasignarEquipoAeronaveAsync(DesasignarEquipoDto dto);

        // ========== GESTIÓN DE ESTADOS ==========
Task ActualizarEstadosEquiposAsync();
        Task<OperationResult<string>> MarcarEquipoEnServicioAsync(int idEquipo);
        Task<OperationResult<string>> MarcarEquipoEnDescansoAsync(int idEquipo, DateTime finVuelo);
    }
}
