using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IVueloAdminService
    {
        Task<OperationResult<VueloDetalleDto>> CrearVueloAsync(CrearVueloDto dto);
        Task<VueloDetalleDto?> ObtenerVueloDetalleAsync(int id);
        Task<OperationResult<VueloDetalleDto>> ActualizarVueloAsync(ActualizarVueloDto dto);
        Task<bool> EliminarVueloAsync(int id);
        Task<List<VueloDetalleDto>> ObtenerTodosLosVuelosAsync();
    }
}