using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface ITicketSoporteService
    {
        Task<TicketSoporteResponseDto> CrearTicketAsync(CrearTicketDto dto);
        Task<List<TicketSoporteResponseDto>> ObtenerTicketsPorClienteAsync(int idCliente);
        Task<List<TicketSoporteResponseDto>> ObtenerTicketsAbiertosAsync();
        Task<bool> ActualizarEstadoTicketAsync(ActualizarTicketDto dto);
    }
}