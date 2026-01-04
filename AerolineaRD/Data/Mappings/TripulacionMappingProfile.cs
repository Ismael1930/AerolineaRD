using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AutoMapper;

namespace AerolineaRD.Data.Mappings
{
    public class TripulacionMappingProfile : Profile
    {
      public TripulacionMappingProfile()
    {
            // ========== PERSONAL ==========
  CreateMap<Personal, PersonalDto>();
            
       CreateMap<CrearPersonalDto, Personal>()
         .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.Estado, opt => opt.Ignore())
        .ForMember(dest => dest.UltimoVueloFin, opt => opt.Ignore())
     .ForMember(dest => dest.FechaContratacion, opt => opt.Ignore())
          .ForMember(dest => dest.Activo, opt => opt.Ignore())
    .ForMember(dest => dest.EquiposPersonal, opt => opt.Ignore());

// ========== EQUIPO ==========
 CreateMap<Equipo, EquipoDto>()
    .ForMember(dest => dest.Miembros, opt => opt.Ignore())
     .ForMember(dest => dest.AsignacionActual, opt => opt.Ignore());

            CreateMap<Equipo, EquipoDetalleDto>()
      .ForMember(dest => dest.Miembros, opt => opt.Ignore())
     .ForMember(dest => dest.Piloto, opt => opt.Ignore())
         .ForMember(dest => dest.Copiloto, opt => opt.Ignore())
       .ForMember(dest => dest.SobrecargoJefe, opt => opt.Ignore())
        .ForMember(dest => dest.Sobrecargos, opt => opt.Ignore())
       .ForMember(dest => dest.EsEquipoCompleto, opt => opt.Ignore())
  .ForMember(dest => dest.MensajeValidacion, opt => opt.Ignore())
         .ForMember(dest => dest.AsignacionActual, opt => opt.Ignore());

// ========== AERONAVE INFO DTO ==========
      CreateMap<Aeronave, AeronaveInfoDto>();
        }
    }
}
