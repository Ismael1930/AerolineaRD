using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IMapper _mapper;

        public NotificacionService(INotificacionRepository notificacionRepository, IMapper mapper)
        {
            _notificacionRepository = notificacionRepository;
            _mapper = mapper;
        }

        public async Task EnviarNotificacionAsync(int idCliente, string tipo, string mensaje)
        {
            var notificacion = new Notificacion
            {
                IdCliente = idCliente,
                Tipo = tipo,
                Mensaje = mensaje,
                FechaEnvio = DateTime.Now,
                Leida = false
            };

            await _notificacionRepository.AddAsync(notificacion);
            await _notificacionRepository.SaveAsync();

            // NO hay envío real de correo
        }

        public async Task<List<NotificacionResponseDto>> ObtenerNotificacionesPorClienteAsync(int idCliente)
        {
            var notificaciones = await _notificacionRepository.ObtenerPorClienteAsync(idCliente);
            return _mapper.Map<List<NotificacionResponseDto>>(notificaciones);
        }

        public async Task<bool> MarcarComoLeidaAsync(int idNotificacion)
        {
            var notificacion = await _notificacionRepository.GetByIdAsync(idNotificacion);
            if (notificacion == null)
                return false;

            notificacion.Leida = true;
            _notificacionRepository.Update(notificacion);
            await _notificacionRepository.SaveAsync();

            return true;
        }
    }
}