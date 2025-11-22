using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IVueloService
    {
        Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros);
        Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id);
        Task<List<AsientoDisponibleDto>> ObtenerAsientosDisponiblesAsync(int idVuelo, string clase);
    }
}