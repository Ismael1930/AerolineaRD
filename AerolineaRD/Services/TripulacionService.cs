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

        public async Task<List<TripulacionDto>> ObtenerPorRolAsync(string rol)
        {
            var tripulacion = await _tripulacionRepository.ObtenerPorRolAsync(rol);
            return _mapper.Map<List<TripulacionDto>>(tripulacion);
        }
    }
}