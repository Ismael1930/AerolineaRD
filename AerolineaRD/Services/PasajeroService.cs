using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class PasajeroService : IPasajeroService
    {
        private readonly IPasajeroRepository _pasajeroRepository;
        private readonly IMapper _mapper;

        public PasajeroService(IPasajeroRepository pasajeroRepository, IMapper mapper)
        {
            _pasajeroRepository = pasajeroRepository;
            _mapper = mapper;
        }

        public async Task<PasajeroResponseDto> CrearPasajeroAsync(CrearPasajeroDto dto)
        {
            // Verificar si ya existe el pasaporte
            var existente = await _pasajeroRepository.ObtenerPorPasaporteAsync(dto.Pasaporte!);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un pasajero con ese número de pasaporte.");

            var pasajero = _mapper.Map<Pasajero>(dto);
            await _pasajeroRepository.AddAsync(pasajero);
            await _pasajeroRepository.SaveAsync();

            return _mapper.Map<PasajeroResponseDto>(pasajero);
        }

        public async Task<PasajeroResponseDto?> ObtenerPorIdAsync(int id)
        {
            var pasajero = await _pasajeroRepository.GetByIdAsync(id);
            return pasajero != null ? _mapper.Map<PasajeroResponseDto>(pasajero) : null;
        }

        public async Task<List<PasajeroResponseDto>> ObtenerTodosAsync()
        {
            var pasajeros = await _pasajeroRepository.GetAllAsync();
            return _mapper.Map<List<PasajeroResponseDto>>(pasajeros);
        }
    }
}