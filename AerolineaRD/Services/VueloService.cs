using AerolineaRD.Data.DTOs;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class VueloService : IVueloService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IMapper _mapper;

        public VueloService(IVueloRepository vueloRepository, IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _mapper = mapper;
        }

        public async Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros)
        {
            var vuelos = await _vueloRepository.BuscarVuelosConFiltrosAsync(
                filtros.Origen, 
                filtros.Destino, 
                filtros.FechaSalida,
                filtros.FechaRegreso,
                filtros.Clase,
                filtros.TipoViaje
            );

            return _mapper.Map<List<VueloResponseDto>>(vuelos);
        }

        public async Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);
            
            if (vuelo == null)
                return null;

            return _mapper.Map<VueloResponseDto>(vuelo);
        }
    }
}