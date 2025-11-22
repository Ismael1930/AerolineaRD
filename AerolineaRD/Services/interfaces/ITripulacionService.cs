using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface ITripulacionService
    {
        Task<TripulacionDto> CrearTripulacionAsync(CrearTripulacionDto dto);
        Task<List<TripulacionDto>> ObtenerTodasAsync();
        Task<TripulacionDto?> ObtenerPorIdAsync(int id);
        Task<List<TripulacionDto>> ObtenerPorRolAsync(string rol);
        Task<TripulacionDto> ActualizarTripulacionAsync(TripulacionDto dto);
        Task<bool> EliminarTripulacionAsync(int id);
    }
}