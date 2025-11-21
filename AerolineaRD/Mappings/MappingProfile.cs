using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AutoMapper;

namespace AerolineaRD.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Aeropuerto a AeropuertoDto
            CreateMap<Aeropuerto, AeropuertoDto>().ReverseMap();

            // Mapeo de Vuelo a VueloResponseDto
            CreateMap<Vuelo, VueloResponseDto>()
                .ForMember(dest => dest.HoraSalida, opt => opt.MapFrom(src => src.HoraSalida))
                .ForMember(dest => dest.HoraLlegada, opt => opt.MapFrom(src => src.HoraLlegada))
                .ForMember(dest => dest.Duracion, opt => opt.MapFrom(src => src.Duracion))
                .ForMember(dest => dest.PrecioBase, opt => opt.MapFrom(src => src.PrecioBase))
                .ForMember(dest => dest.TipoVuelo, opt => opt.MapFrom(src => src.TipoVuelo))
                .ForMember(dest => dest.FechaRegreso, opt => opt.MapFrom(src => src.FechaRegreso))
                .ForMember(dest => dest.OrigenNombre, opt => opt.MapFrom(src => src.Origen != null ? src.Origen.Nombre : null))
                .ForMember(dest => dest.OrigenCiudad, opt => opt.MapFrom(src => src.Origen != null ? src.Origen.Ciudad : null))
                .ForMember(dest => dest.DestinoNombre, opt => opt.MapFrom(src => src.Destino != null ? src.Destino.Nombre : null))
                .ForMember(dest => dest.DestinoCiudad, opt => opt.MapFrom(src => src.Destino != null ? src.Destino.Ciudad : null))
                .ForMember(dest => dest.AsientosDisponibles, opt => opt.MapFrom(src => src.Asientos.Count(a => a.Disponibilidad == "Disponible")))
                .ForMember(dest => dest.ClasesDisponibles, opt => opt.MapFrom(src => src.Asientos
                    .Where(a => a.Disponibilidad == "Disponible")
                    .Select(a => a.Clase)
                    .Distinct()
                    .ToList()));
        }
    }
}