using AerolineaRD.Data.DTOs;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Services
{
    public class AeropuertoService : IAeropuertoService
    {
        private readonly IAeropuertoRepository _aeropuertoRepository;
        private readonly IVueloRepository _vueloRepository;
        private readonly IMapper _mapper;

        public AeropuertoService(
            IAeropuertoRepository aeropuertoRepository,
            IVueloRepository vueloRepository,
            IMapper mapper)
        {
            _aeropuertoRepository = aeropuertoRepository;
            _vueloRepository = vueloRepository;
            _mapper = mapper;
        }

        public async Task<List<AeropuertoDto>> ObtenerTodosAsync()
        {
            var aeropuertos = await _aeropuertoRepository.ObtenerTodosOrdenadosAsync();
            return _mapper.Map<List<AeropuertoDto>>(aeropuertos);
        }

        public async Task<AeropuertoCapacidadDto> ObtenerCapacidadAeropuertoAsync(string codigoAeropuerto, DateTime fecha)
        {
            var aeropuerto = await _aeropuertoRepository.GetByIdAsync(codigoAeropuerto);
            if (aeropuerto == null)
                throw new KeyNotFoundException($"Aeropuerto '{codigoAeropuerto}' no encontrado");

            // ? Crear rango de fechas para la consulta (sin usar .Date en LINQ to SQL)
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            // Obtener todos los vuelos del aeropuerto en la fecha especificada
            var todosLosVuelos = await _vueloRepository.GetAllAsync();
  
            // ? Traer a memoria y luego filtrar por fecha
            var vuelosSalida = todosLosVuelos
                .Where(v => v.OrigenCodigo == codigoAeropuerto
      && v.Fecha >= fechaInicio && v.Fecha < fechaFin
    && v.Estado != "Cancelado")
                .ToList();

            var vuelosLlegada = todosLosVuelos
                .Where(v => v.DestinoCodigo == codigoAeropuerto
       && v.Fecha >= fechaInicio && v.Fecha < fechaFin
&& v.Estado != "Cancelado")
 .ToList();

            // Calcular uso por hora (0-23)
            var usoPorHora = new List<UsoHorarioDto>();
            for (int hora = 0; hora < 24; hora++)
            {
                var horaInicio = new TimeSpan(hora, 0, 0);
                var horaFin = new TimeSpan(hora, 59, 59);

                var salidas = vuelosSalida.Count(v => v.HoraSalida >= horaInicio && v.HoraSalida <= horaFin);
                var llegadas = vuelosLlegada.Count(v => v.HoraLlegada >= horaInicio && v.HoraLlegada <= horaFin);

                var capacidad = aeropuerto.CapacidadVuelosPorHora;
                var porcentajeSalida = capacidad > 0 ? (decimal)salidas / capacidad * 100 : 0;
                var porcentajeLlegada = capacidad > 0 ? (decimal)llegadas / capacidad * 100 : 0;

                usoPorHora.Add(new UsoHorarioDto
                {
                    Hora = hora,
                    HoraFormato = $"{hora:00}:00 - {hora:00}:59",
                    VuelosSalida = salidas,
                    VuelosLlegada = llegadas,
                    TotalVuelos = salidas + llegadas,
                    CapacidadDisponibleSalida = Math.Max(0, capacidad - salidas),
                    CapacidadDisponibleLlegada = Math.Max(0, capacidad - llegadas),
                    PorcentajeUsoSalida = Math.Round(porcentajeSalida, 2),
                    PorcentajeUsoLlegada = Math.Round(porcentajeLlegada, 2),
                    SobreCapacidadSalida = salidas > capacidad,
                    SobreCapacidadLlegada = llegadas > capacidad
                });
            }

   var totalSalidas = vuelosSalida.Count;
  var totalLlegadas = vuelosLlegada.Count;
  var capacidadDiaria = aeropuerto.CapacidadVuelosPorHora * 24;

     return new AeropuertoCapacidadDto
   {
  Codigo = aeropuerto.Codigo,
   Nombre = aeropuerto.Nombre,
         Ciudad = aeropuerto.Ciudad,
   Pais = aeropuerto.Pais,
CapacidadPorHora = aeropuerto.CapacidadVuelosPorHora,
    TotalVuelosSalida = totalSalidas,
    TotalVuelosLlegada = totalLlegadas,
    TotalVuelos = totalSalidas + totalLlegadas,
      UsoPorHora = usoPorHora,
    PorcentajeUsoSalidas = capacidadDiaria > 0 ? Math.Round((decimal)totalSalidas / capacidadDiaria * 100, 2) : 0,
      PorcentajeUsoLlegadas = capacidadDiaria > 0 ? Math.Round((decimal)totalLlegadas / capacidadDiaria * 100, 2) : 0,
   PorcentajeUsoTotal = capacidadDiaria > 0 ? Math.Round((decimal)(totalSalidas + totalLlegadas) / (capacidadDiaria * 2) * 100, 2) : 0
 };
        }

        public async Task<ReporteCapacidadAeropuertosDto> ObtenerReporteCapacidadTodosAsync(DateTime fecha)
        {
            var aeropuertos = await _aeropuertoRepository.ObtenerTodosOrdenadosAsync();
            var reportes = new List<AeropuertoCapacidadDto>();

            foreach (var aeropuerto in aeropuertos)
            {
                var capacidad = await ObtenerCapacidadAeropuertoAsync(aeropuerto.Codigo, fecha);
                reportes.Add(capacidad);
            }

            var aeropuertosSobreCapacidad = reportes.Count(r =>
                r.UsoPorHora.Any(u => u.SobreCapacidadSalida || u.SobreCapacidadLlegada));

            return new ReporteCapacidadAeropuertosDto
            {
                FechaConsulta = fecha,
                Aeropuertos = reportes.OrderByDescending(r => r.PorcentajeUsoTotal).ToList(),
                TotalAeropuertos = reportes.Count,
                AeropuertosSobreCapacidad = aeropuertosSobreCapacidad
            };
        }
    }
}