using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class TripulacionService : ITripulacionService
    {
        private readonly ITripulacionRepository _tripulacionRepository;
        private readonly IMapper _mapper;

        public TripulacionService(ITripulacionRepository tripulacionRepository, IMapper mapper)
        {
            _tripulacionRepository = tripulacionRepository;
            _mapper = mapper;
        }

        public async Task<TripulacionDto> CrearTripulacionAsync(CrearTripulacionDto dto)
        {
            var tripulacion = _mapper.Map<Tripulacion>(dto);
            await _tripulacionRepository.AddAsync(tripulacion);
            await _tripulacionRepository.SaveAsync();

            return _mapper.Map<TripulacionDto>(tripulacion);
        }

        public async Task<List<TripulacionDto>> ObtenerTodasAsync()
        {
            var tripulacion = await _tripulacionRepository.GetAllAsync();
            return _mapper.Map<List<TripulacionDto>>(tripulacion);
        }

        public async Task<TripulacionDto?> ObtenerPorIdAsync(int id)
        {
            var tripulacion = await _tripulacionRepository.GetByIdAsync(id);
            return tripulacion != null ? _mapper.Map<TripulacionDto>(tripulacion) : null;
        }

        public async Task<List<TripulacionDto>> ObtenerPorRolAsync(string rol)
        {
            var tripulacion = await _tripulacionRepository.ObtenerPorRolAsync(rol);
            return _mapper.Map<List<TripulacionDto>>(tripulacion);
        }

        public async Task<TripulacionDto> ActualizarTripulacionAsync(TripulacionDto dto)
        {
            var tripulacion = await _tripulacionRepository.GetByIdAsync(dto.Id);
            if (tripulacion == null)
                throw new KeyNotFoundException($"Tripulación con ID {dto.Id} no encontrada.");

            // Actualizar solo los campos que vienen en el DTO
            if (!string.IsNullOrEmpty(dto.Nombre)) tripulacion.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) tripulacion.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Rol)) tripulacion.Rol = dto.Rol;
            if (!string.IsNullOrEmpty(dto.Licencia)) tripulacion.Licencia = dto.Licencia;

            _tripulacionRepository.Update(tripulacion);
            await _tripulacionRepository.SaveAsync();

            return _mapper.Map<TripulacionDto>(tripulacion);
        }

        public async Task<bool> EliminarTripulacionAsync(int id)
        {
            var tripulacion = await _tripulacionRepository.GetByIdAsync(id);
            if (tripulacion == null)
                return false;

            _tripulacionRepository.Delete(tripulacion);
            await _tripulacionRepository.SaveAsync();

            return true;
        }
    }
}