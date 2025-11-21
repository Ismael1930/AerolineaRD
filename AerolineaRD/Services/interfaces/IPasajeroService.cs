using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IPasajeroService
    {
        Task<PasajeroResponseDto> CrearPasajeroAsync(CrearPasajeroDto dto);
        Task<PasajeroResponseDto?> ObtenerPorIdAsync(int id);
        Task<List<PasajeroResponseDto>> ObtenerTodosAsync();
    }
}