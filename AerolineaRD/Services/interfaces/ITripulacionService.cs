using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface ITripulacionService
    {
        Task<TripulacionDto> CrearTripulacionAsync(CrearTripulacionDto dto);
        Task<List<TripulacionDto>> ObtenerPorRolAsync(string rol);
    }
}