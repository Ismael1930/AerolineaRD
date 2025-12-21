using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Services
{
    public class AeronaveService : IAeronaveService
    {
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly IVueloRepository _vueloRepository;
        private readonly IMapper _mapper;

        public AeronaveService(
          IAeronaveRepository aeronaveRepository,
  IVueloRepository vueloRepository,
            IMapper mapper)
        {
_aeronaveRepository = aeronaveRepository;
      _vueloRepository = vueloRepository;
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

        public async Task<List<AeronaveConDisponibilidadDto>> ObtenerTodasConDisponibilidadAsync()
        {
    var aeronaves = await _aeronaveRepository.GetAllAsync();
   var resultado = new List<AeronaveConDisponibilidadDto>();

  foreach (var aeronave in aeronaves)
            {
           var disponibilidad = await CalcularDisponibilidadAsync(aeronave);
         resultado.Add(disponibilidad);
    }

   return resultado;
        }

     public async Task<AeronaveConDisponibilidadDto?> ObtenerConDisponibilidadAsync(string matricula)
        {
            var aeronave = await _aeronaveRepository.GetByIdAsync(matricula);
       if (aeronave == null)
       return null;

       return await CalcularDisponibilidadAsync(aeronave);
        }

        private async Task<AeronaveConDisponibilidadDto> CalcularDisponibilidadAsync(Aeronave aeronave)
   {
    // Obtener todos los vuelos de esta aeronave con sus asientos y reservas
         var todosVuelos = await _vueloRepository.GetAllAsync();
            var vuelosAeronave = todosVuelos.Where(v => v.Matricula == aeronave.Matricula 
       && v.Estado != "Cancelado").ToList();

     // Cargar detalles completos de los vuelos
     var vuelosDetallados = new List<Vuelo>();
       foreach (var vuelo in vuelosAeronave)
   {
     var vueloDetalle = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);
    if (vueloDetalle != null)
    {
    vuelosDetallados.Add(vueloDetalle);
  }
}

            // Contar asientos por clase
        var asientos = aeronave.Asientos?.ToList() ?? new List<Asiento>();
      
       var primeraTotal = asientos.Count(a => a.Clase == "Primera");
var ejecutivaTotal = asientos.Count(a => a.Clase == "Ejecutiva");
   var economicaTotal = asientos.Count(a => a.Clase == "Economica");

         // Contar reservas por clase en todos los vuelos
    var todasReservas = vuelosDetallados
        .SelectMany(v => v.Reservas ?? Enumerable.Empty<Reserva>())
     .Where(r => r.Estado == "Confirmada")
    .ToList();

    var primeraReservados = todasReservas.Count(r => r.Clase == "Primera");
      var ejecutivaReservados = todasReservas.Count(r => r.Clase == "Ejecutiva");
          var economicaReservados = todasReservas.Count(r => r.Clase == "Economica");

          // Calcular disponibles
    var primeraDisponibles = primeraTotal - primeraReservados;
  var ejecutivaDisponibles = ejecutivaTotal - ejecutivaReservados;
            var economicaDisponibles = economicaTotal - economicaReservados;

 var totalAsientos = asientos.Count;
       var totalReservados = todasReservas.Count;
       var totalDisponibles = totalAsientos - totalReservados;

      // Calcular porcentajes
         decimal primeraPorcentaje = primeraTotal > 0 ? (decimal)primeraReservados / primeraTotal * 100 : 0;
      decimal ejecutivaPorcentaje = ejecutivaTotal > 0 ? (decimal)ejecutivaReservados / ejecutivaTotal * 100 : 0;
     decimal economicaPorcentaje = economicaTotal > 0 ? (decimal)economicaReservados / economicaTotal * 100 : 0;
         decimal totalPorcentaje = totalAsientos > 0 ? (decimal)totalReservados / totalAsientos * 100 : 0;

         // Contar vuelos
            var vuelosHoy = vuelosDetallados.Count(v => v.Fecha.Date == DateTime.Today);

return new AeronaveConDisponibilidadDto
    {
   Matricula = aeronave.Matricula,
            Modelo = aeronave.Modelo,
         Capacidad = aeronave.Capacidad,
   Estado = aeronave.Estado,
      TiempoPreparacionMinutos = aeronave.TiempoPreparacionMinutos,
 TotalAsientos = totalAsientos,
    TotalVuelosProgramados = vuelosDetallados.Count,
 VuelosHoy = vuelosHoy,
  DisponibilidadAsientos = new DisponibilidadAsientosDto
      {
      PrimeraTotal = primeraTotal,
   PrimeraReservados = primeraReservados,
PrimeraDisponibles = Math.Max(0, primeraDisponibles),
 PrimeraPorcentajeOcupacion = Math.Round(primeraPorcentaje, 2),

                 EjecutivaTotal = ejecutivaTotal,
    EjecutivaReservados = ejecutivaReservados,
        EjecutivaDisponibles = Math.Max(0, ejecutivaDisponibles),
   EjecutivaPorcentajeOcupacion = Math.Round(ejecutivaPorcentaje, 2),

 EconomicaTotal = economicaTotal,
    EconomicaReservados = economicaReservados,
       EconomicaDisponibles = Math.Max(0, economicaDisponibles),
            EconomicaPorcentajeOcupacion = Math.Round(economicaPorcentaje, 2),

      Total = totalAsientos,
        TotalReservados = totalReservados,
          TotalDisponibles = Math.Max(0, totalDisponibles),
        PorcentajeOcupacionTotal = Math.Round(totalPorcentaje, 2)
          }
    };
        }
  }
}