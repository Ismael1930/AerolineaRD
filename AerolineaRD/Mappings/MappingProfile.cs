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
            CreateMap<Aeropuerto, AeropuertoDto>();

            // Mapeo de Aeronave a AeronaveResponseDto
            CreateMap<Aeronave, AeronaveResponseDto>();
            CreateMap<CrearAeronaveDto, Aeronave>();

            // Mapeo de Vuelo a VueloResponseDto
            CreateMap<Vuelo, VueloResponseDto>()
                .ForMember(dest => dest.OrigenNombre, opt => opt.MapFrom(src => src.Origen.Nombre))
                .ForMember(dest => dest.OrigenCiudad, opt => opt.MapFrom(src => src.Origen.Ciudad))
                .ForMember(dest => dest.DestinoNombre, opt => opt.MapFrom(src => src.Destino.Nombre))
                .ForMember(dest => dest.DestinoCiudad, opt => opt.MapFrom(src => src.Destino.Ciudad))
                .ForMember(dest => dest.ClasesDisponibles, opt => opt.Ignore()); // Se calcula manualmente

            // Mapeo de Vuelo a VueloDetalleDto (hereda de VueloResponseDto)
            CreateMap<Vuelo, VueloDetalleDto>()
                .IncludeBase<Vuelo, VueloResponseDto>()
                .ForMember(dest => dest.Tripulacion, opt => opt.MapFrom(src => src.Tripulaciones))
                .ForMember(dest => dest.EstadoActual, opt => opt.MapFrom(src => src.EstadoVueloDetalle));

            // Mapeo de CrearVueloDto a Vuelo
            CreateMap<CrearVueloDto, Vuelo>()
                .ForMember(dest => dest.Tripulaciones, opt => opt.Ignore()); // Se asigna manualmente en el servicio

            // Mapeo de Tripulacion a TripulacionDto
            CreateMap<Tripulacion, TripulacionDto>();
            CreateMap<CrearTripulacionDto, Tripulacion>();

            // Mapeo de EstadoVuelo a EstadoVueloDto
            CreateMap<EstadoVuelo, EstadoVueloDto>();

            // Mapeo de Factura a FacturaResponseDto
            CreateMap<Factura, FacturaResponseDto>();

            // Mapeo de Reserva a ReservaResponseDto
            CreateMap<Reserva, ReservaResponseDto>()
                .ForMember(dest => dest.PasajeroNombre, opt => opt.MapFrom(src => src.Pasajero.Nombre))
                .ForMember(dest => dest.PasajeroApellido, opt => opt.MapFrom(src => src.Pasajero.Apellido))
                .ForMember(dest => dest.NumeroVuelo, opt => opt.MapFrom(src => src.Vuelo.NumeroVuelo))
                .ForMember(dest => dest.FechaVuelo, opt => opt.MapFrom(src => src.Vuelo.Fecha))
                .ForMember(dest => dest.Origen, opt => opt.MapFrom(src => src.Vuelo.Origen.Ciudad))
                .ForMember(dest => dest.Destino, opt => opt.MapFrom(src => src.Vuelo.Destino.Ciudad))
                .ForMember(dest => dest.Factura, opt => opt.MapFrom(src => src.Factura)); // Mapear Factura
        }
    }
}