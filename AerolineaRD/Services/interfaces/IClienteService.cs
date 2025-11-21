using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> ObtenerTodosAsync();
        Task<ClienteDto?> ObtenerPorIdAsync(int id);
        Task<ClienteConReservasDto?> ObtenerClienteConReservasAsync(int id);
        Task<ClienteDto?> ObtenerPorUserIdAsync(string userId);
        Task<ClienteDto?> ObtenerPorEmailAsync(string email);
        Task<ClienteDto> CrearAsync(CrearClienteDto dto);
        Task<ClienteDto> ActualizarAsync(int id, ActualizarClienteDto dto);
        Task<bool> EliminarAsync(int id);
    }
}