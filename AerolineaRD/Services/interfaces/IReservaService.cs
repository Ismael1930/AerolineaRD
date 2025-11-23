using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IReservaService
    {
        Task<ReservaResponseDto> CrearReservaAsync(CrearReservaDto dto);
        Task<List<ReservaResponseDto>> ObtenerTodasAsync();
        Task<ReservaResponseDto?> ObtenerReservaPorCodigoAsync(string codigo);
        Task<List<ReservaResponseDto>> ObtenerReservasPorClienteAsync(int idCliente);
        Task<ReservaResponseDto> ModificarReservaAsync(ModificarReservaDto dto);
        Task<bool> CancelarReservaAsync(string codigo);
    }
}