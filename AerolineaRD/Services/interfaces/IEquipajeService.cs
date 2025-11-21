using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IEquipajeService
    {
        Task<EquipajeResponseDto> RegistrarEquipajeAsync(CrearEquipajeDto dto);
        Task<List<EquipajeResponseDto>> ObtenerEquipajesPorPasajeroAsync(int idPasajero);
    }
}