using AerolineaRD.Data.DTOs;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class AeropuertoService : IAeropuertoService
    {
        private readonly IAeropuertoRepository _aeropuertoRepository;
        private readonly IMapper _mapper;

        public AeropuertoService(IAeropuertoRepository aeropuertoRepository, IMapper mapper)
        {
            _aeropuertoRepository = aeropuertoRepository;
            _mapper = mapper;
        }

        public async Task<List<AeropuertoDto>> ObtenerTodosAsync()
        {
            var aeropuertos = await _aeropuertoRepository.ObtenerTodosOrdenadosAsync();
            return _mapper.Map<List<AeropuertoDto>>(aeropuertos);
        }
    }
}