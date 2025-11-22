using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class VueloAdminService : IVueloAdminService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IEstadoVueloRepository _estadoVueloRepository;
        private readonly ITripulacionRepository _tripulacionRepository;
        private readonly IMapper _mapper;

        public VueloAdminService(
            IVueloRepository vueloRepository,
            IEstadoVueloRepository estadoVueloRepository,
            ITripulacionRepository tripulacionRepository,
            IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _estadoVueloRepository = estadoVueloRepository;
            _tripulacionRepository = tripulacionRepository;
            _mapper = mapper;
        }

        public async Task<VueloDetalleDto> CrearVueloAsync(CrearVueloDto dto)
        {
            var vuelo = _mapper.Map<Vuelo>(dto);
            vuelo.Estado = "Programado";

            // Asignar tripulación
            if (dto.IdsTripulacion != null && dto.IdsTripulacion.Any())
            {
                foreach (var idTripulacion in dto.IdsTripulacion)
                {
                    var tripulacion = await _tripulacionRepository.GetByIdAsync(idTripulacion);
                    if (tripulacion != null)
                    {
                        vuelo.Tripulaciones.Add(tripulacion);
                    }
                }
            }

            await _vueloRepository.AddAsync(vuelo);
            await _vueloRepository.SaveAsync();

            // Crear estado inicial del vuelo
            var estadoVuelo = new EstadoVuelo
            {
                IdVuelo = vuelo.Id,
                Estado = "Programado",
                HoraSalidaProgramada = dto.Fecha.Date.Add(dto.HoraSalida),
                HoraLlegadaProgramada = dto.Fecha.Date.Add(dto.HoraLlegada)
            };

            await _estadoVueloRepository.AddAsync(estadoVuelo);
            await _estadoVueloRepository.SaveAsync();

            var vueloCreado = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);
            return _mapper.Map<VueloDetalleDto>(vueloCreado);
        }

        public async Task<VueloDetalleDto?> ObtenerVueloDetalleAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);
            return vuelo != null ? _mapper.Map<VueloDetalleDto>(vuelo) : null;
        }

        public async Task<VueloDetalleDto> ActualizarVueloAsync(ActualizarVueloDto dto)
        {
            var vuelo = await _vueloRepository.GetByIdAsync(dto.Id);
            if (vuelo == null)
                throw new KeyNotFoundException("Vuelo no encontrado.");

            // Actualizar campos básicos
            if (!string.IsNullOrEmpty(dto.NumeroVuelo)) vuelo.NumeroVuelo = dto.NumeroVuelo;
            if (dto.Fecha.HasValue) vuelo.Fecha = dto.Fecha.Value;
            if (dto.HoraSalida.HasValue) vuelo.HoraSalida = dto.HoraSalida.Value;
            if (dto.HoraLlegada.HasValue) vuelo.HoraLlegada = dto.HoraLlegada.Value;
            if (dto.Duracion.HasValue) vuelo.Duracion = dto.Duracion.Value;
            if (dto.PrecioBase.HasValue) vuelo.PrecioBase = dto.PrecioBase.Value;
            if (!string.IsNullOrEmpty(dto.OrigenCodigo)) vuelo.OrigenCodigo = dto.OrigenCodigo;
            if (!string.IsNullOrEmpty(dto.DestinoCodigo)) vuelo.DestinoCodigo = dto.DestinoCodigo;
            if (!string.IsNullOrEmpty(dto.Matricula)) vuelo.Matricula = dto.Matricula;
            if (!string.IsNullOrEmpty(dto.Estado)) vuelo.Estado = dto.Estado;
            if (!string.IsNullOrEmpty(dto.TipoVuelo)) vuelo.TipoVuelo = dto.TipoVuelo;

            _vueloRepository.Update(vuelo);
            await _vueloRepository.SaveAsync();

            var vueloActualizado = await _vueloRepository.ObtenerVueloConDetallesAsync(dto.Id);
            return _mapper.Map<VueloDetalleDto>(vueloActualizado);
        }

        public async Task<bool> EliminarVueloAsync(int id)
        {
            var vuelo = await _vueloRepository.GetByIdAsync(id);
            if (vuelo == null)
                return false;

            _vueloRepository.Delete(vuelo);
            await _vueloRepository.SaveAsync();

            return true;
        }

        public async Task<List<VueloDetalleDto>> ObtenerTodosLosVuelosAsync()
        {
            var vuelos = await _vueloRepository.GetAllAsync();
            return _mapper.Map<List<VueloDetalleDto>>(vuelos);
        }
    }
}