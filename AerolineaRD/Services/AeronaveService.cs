using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class AeronaveService : IAeronaveService
    {
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly IMapper _mapper;

        public AeronaveService(IAeronaveRepository aeronaveRepository, IMapper mapper)
        {
            _aeronaveRepository = aeronaveRepository;
            _mapper = mapper;
        }

        public async Task<AeronaveResponseDto> CrearAeronaveAsync(CrearAeronaveDto dto)
        {
            var aeronave = _mapper.Map<Aeronave>(dto);
            await _aeronaveRepository.AddAsync(aeronave);
            await _aeronaveRepository.SaveAsync();

            return _mapper.Map<AeronaveResponseDto>(aeronave);
        }

        public async Task<List<AeronaveResponseDto>> ObtenerAeronavesDisponiblesAsync()
        {
            var aeronaves = await _aeronaveRepository.ObtenerAeronavesDisponiblesAsync();
            return _mapper.Map<List<AeronaveResponseDto>>(aeronaves);
        }

        public async Task<List<AeronaveResponseDto>> ObtenerTodasAsync()
        {
            var aeronaves = await _aeronaveRepository.GetAllAsync();
            return _mapper.Map<List<AeronaveResponseDto>>(aeronaves);
        }

        public async Task<AeronaveResponseDto?> ObtenerPorMatriculaAsync(string matricula)
        {
            var aeronave = await _aeronaveRepository.GetByIdAsync(matricula);
            return aeronave != null ? _mapper.Map<AeronaveResponseDto>(aeronave) : null;
        }

        public async Task<AeronaveResponseDto> ActualizarAeronaveAsync(ActualizarAeronaveDto dto)
        {
            var aeronave = await _aeronaveRepository.GetByIdAsync(dto.Matricula);
            if (aeronave == null)
                throw new KeyNotFoundException($"Aeronave con matrícula {dto.Matricula} no encontrada.");

            // Actualizar solo los campos que vienen en el DTO
            if (!string.IsNullOrEmpty(dto.Modelo)) aeronave.Modelo = dto.Modelo;
            if (dto.Capacidad.HasValue) aeronave.Capacidad = dto.Capacidad.Value;
            if (!string.IsNullOrEmpty(dto.Estado)) aeronave.Estado = dto.Estado;

            _aeronaveRepository.Update(aeronave);
            await _aeronaveRepository.SaveAsync();

            return _mapper.Map<AeronaveResponseDto>(aeronave);
        }

        public async Task<bool> EliminarAeronaveAsync(string matricula)
        {
            var aeronave = await _aeronaveRepository.GetByIdAsync(matricula);
            if (aeronave == null)
                return false;

            _aeronaveRepository.Delete(aeronave);
            await _aeronaveRepository.SaveAsync();

            return true;
        }
    }
}