using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IEstadoVueloService
    {
        Task<EstadoVueloDto?> ObtenerEstadoPorVueloAsync(int idVuelo);
        Task<EstadoVueloDto> ActualizarEstadoAsync(ActualizarEstadoVueloDto dto);
    }
}