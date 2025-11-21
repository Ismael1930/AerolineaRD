using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface INotificacionService
    {
        Task EnviarNotificacionAsync(int idCliente, string tipo, string mensaje);
        Task<List<NotificacionResponseDto>> ObtenerNotificacionesPorClienteAsync(int idCliente);
        Task<bool> MarcarComoLeidaAsync(int idNotificacion);
    }
}