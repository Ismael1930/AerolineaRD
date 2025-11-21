using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IAeronaveService
    {
        Task<AeronaveResponseDto> CrearAeronaveAsync(CrearAeronaveDto dto);
        Task<List<AeronaveResponseDto>> ObtenerAeronavesDisponiblesAsync();
    }
}