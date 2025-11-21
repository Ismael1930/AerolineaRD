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
    }
}