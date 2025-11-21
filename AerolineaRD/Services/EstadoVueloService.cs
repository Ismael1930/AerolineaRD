using AerolineaRD.Data.DTOs;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class EstadoVueloService : IEstadoVueloService
    {
        private readonly IEstadoVueloRepository _estadoVueloRepository;
        private readonly INotificacionService _notificacionService;
        private readonly IMapper _mapper;

        public EstadoVueloService(
            IEstadoVueloRepository estadoVueloRepository,
            INotificacionService notificacionService,
            IMapper mapper)
        {
            _estadoVueloRepository = estadoVueloRepository;
            _notificacionService = notificacionService;
            _mapper = mapper;
        }

        public async Task<EstadoVueloDto?> ObtenerEstadoPorVueloAsync(int idVuelo)
        {
            var estado = await _estadoVueloRepository.ObtenerPorVueloAsync(idVuelo);
            return estado != null ? _mapper.Map<EstadoVueloDto>(estado) : null;
        }

        public async Task<EstadoVueloDto> ActualizarEstadoAsync(ActualizarEstadoVueloDto dto)
        {
            var estado = await _estadoVueloRepository.ObtenerPorVueloAsync(dto.IdVuelo);
            if (estado == null)
                throw new KeyNotFoundException("Estado de vuelo no encontrado.");

            if (!string.IsNullOrEmpty(dto.Estado)) estado.Estado = dto.Estado;
            if (dto.HoraSalida.HasValue) estado.HoraSalida = dto.HoraSalida;
            if (dto.HoraLlegada.HasValue) estado.HoraLlegada = dto.HoraLlegada;
            if (!string.IsNullOrEmpty(dto.Puerta)) estado.Puerta = dto.Puerta;
            if (!string.IsNullOrEmpty(dto.Observaciones)) estado.Observaciones = dto.Observaciones;

            _estadoVueloRepository.Update(estado);
            await _estadoVueloRepository.SaveAsync();

            // Notificar cambios a pasajeros (lógica simplificada)
            // await _notificacionService.NotificarCambioVueloAsync(dto.IdVuelo, dto.Estado);

            return _mapper.Map<EstadoVueloDto>(estado);
        }
    }
}