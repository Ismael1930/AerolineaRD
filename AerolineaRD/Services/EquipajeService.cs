using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class EquipajeService : IEquipajeService
    {
        private readonly IEquipajeRepository _equipajeRepository;
        private readonly IMapper _mapper;

        public EquipajeService(IEquipajeRepository equipajeRepository, IMapper mapper)
        {
            _equipajeRepository = equipajeRepository;
            _mapper = mapper;
        }

        public async Task<EquipajeResponseDto> RegistrarEquipajeAsync(CrearEquipajeDto dto)
        {
            var equipaje = new Equipaje
            {
                Numero = GenerarNumeroEquipaje(),
                IdPasajero = dto.IdPasajero,
                Peso = dto.Peso,
                Tipo = dto.Tipo
            };

            await _equipajeRepository.AddAsync(equipaje);
            await _equipajeRepository.SaveAsync();

            return _mapper.Map<EquipajeResponseDto>(equipaje);
        }

        public async Task<List<EquipajeResponseDto>> ObtenerEquipajesPorPasajeroAsync(int idPasajero)
        {
            var equipajes = await _equipajeRepository.ObtenerEquipajesPorPasajeroAsync(idPasajero);
            return _mapper.Map<List<EquipajeResponseDto>>(equipajes);
        }

        private string GenerarNumeroEquipaje()
        {
            return "EQ" + DateTime.Now.Ticks.ToString().Substring(9);
        }
    }
}