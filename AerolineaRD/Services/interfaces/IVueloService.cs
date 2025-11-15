
using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IVueloService
    {
        Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros);
        Task<List<AeropuertoDto>> ObtenerAeropuertosAsync();
        Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id);
    }
}