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

        public async Task<AeropuertoCapacidadDto> ObtenerCapacidadAeropuertoAsync(string codigoAeropuerto, DateTime fechaInicio, DateTime fechaFin)
        {
            var aeropuerto = await _aeropuertoRepository.GetByIdAsync(codigoAeropuerto);
            if (aeropuerto == null)
                throw new KeyNotFoundException($"Aeropuerto '{codigoAeropuerto}' no encontrado");

            // ✅ NUEVO: Obtener todos los vuelos en el rango de fechas
            var fechaInicioSolo = fechaInicio.Date;
            var fechaFinSolo = fechaFin.Date.AddDays(1); // Incluir el último día completo

            // Obtener vuelos de salida desde este aeropuerto
            var vuelosSalida = await _vueloRepository.Context.Vuelos
          .Where(v => v.OrigenCodigo == codigoAeropuerto
      && v.Fecha >= fechaInicioSolo
 && v.Fecha < fechaFinSolo
           && v.Estado != "Cancelado")
     .ToListAsync();

            // Obtener vuelos de llegada a este aeropuerto
            var vuelosLlegada = await _vueloRepository.Context.Vuelos
             .Where(v => v.DestinoCodigo == codigoAeropuerto
           && v.Fecha >= fechaInicioSolo
                  && v.Fecha < fechaFinSolo
            && v.Estado != "Cancelado")
               .ToListAsync();

            // ✅ NUEVO: Agregar vuelos de regreso (IdaYVuelta)
            // Si un vuelo es ATL→BCN con regreso, el retorno BCN→ATL se considera automáticamente
            var vuelosIdaYVueltasalenDeAqui = await _vueloRepository.Context.Vuelos
         .Where(v => v.OrigenCodigo == codigoAeropuerto
            && v.TipoVuelo == "IdaYVuelta"
              && v.FechaRegreso.HasValue
        && v.FechaRegreso.Value >= fechaInicioSolo
        && v.FechaRegreso.Value < fechaFinSolo
        && v.Estado != "Cancelado")
             .ToListAsync();

            // ✅ NUEVO: Vuelos de regreso que llegan a este aeropuerto
            var vuelosIdaYVueltaLleganAqui = await _vueloRepository.Context.Vuelos
    .Where(v => v.DestinoCodigo == codigoAeropuerto
        && v.TipoVuelo == "IdaYVuelta"
        && v.FechaRegreso.HasValue
             && v.FechaRegreso.Value >= fechaInicioSolo
   && v.FechaRegreso.Value < fechaFinSolo
        && v.Estado != "Cancelado")
  .ToListAsync();

            // 📝 Crear "vuelos virtuales" para los regresos
            // Vuelo original: ATL→BCN (01/01 10:00)
            // Vuelo virtual: BCN→ATL (05/01 10:00) - mismo horario
            var vuelosRegresoComoSalida = vuelosIdaYVueltaLleganAqui.Select(v => new
            {
                Vuelo = v,
                FechaRegreso = v.FechaRegreso!.Value,
                HoraSalida = v.HoraSalida, // Misma hora de salida original
                EsVueloDeRegreso = true
            }).ToList();

            var vuelosRegresoComoLlegada = vuelosIdaYVueltasalenDeAqui.Select(v => new
            {
                Vuelo = v,
                FechaRegreso = v.FechaRegreso!.Value,
                HoraLlegada = v.HoraLlegada, // Misma hora de llegada original
                EsVueloDeRegreso = true
            }).ToList();

            // ✅ Agrupar vuelos por día (solo días con actividad)
            // Incluir vuelos de ida, llegada Y regresos de IdaYVuelta
            var fechasConVuelos = vuelosSalida.Select(v => v.Fecha.Date)
               .Concat(vuelosLlegada.Select(v => v.Fecha.Date))
             .Concat(vuelosRegresoComoSalida.Select(v => v.FechaRegreso.Date))
           .Concat(vuelosRegresoComoLlegada.Select(v => v.FechaRegreso.Date))
           .Distinct()
              .OrderBy(f => f)
                    .ToList();

            // ✅ Capacidad diaria del aeropuerto
            var capacidadDiaria = aeropuerto.CapacidadVuelosPorHora * 24;

            // ✅ Crear calendario de días con vuelos
            var diasConVuelos = new List<UsoDiarioDto>();

            foreach (var fecha in fechasConVuelos)
            {
                // Contar salidas: vuelos normales + regresos que salen de aquí
                var salidaDelDia = vuelosSalida.Count(v => v.Fecha.Date == fecha)
                       + vuelosRegresoComoSalida.Count(v => v.FechaRegreso.Date == fecha);

                // Contar llegadas: vuelos normales + regresos que llegan aquí
                var llegadaDelDia = vuelosLlegada.Count(v => v.Fecha.Date == fecha)
               + vuelosRegresoComoLlegada.Count(v => v.FechaRegreso.Date == fecha);

                var totalDelDia = salidaDelDia + llegadaDelDia;

                var porcentajeUso = capacidadDiaria > 0
          ? (decimal)totalDelDia / capacidadDiaria * 100
               : 0;

                var sobreCapacidad = totalDelDia > capacidadDiaria;

                // Determinar nivel de alerta
                string nivelAlerta;
                if (porcentajeUso > 100)
                    nivelAlerta = "CRITICO";
                else if (porcentajeUso > 85)
                    nivelAlerta = "ALTO";
                else if (porcentajeUso > 60)
                    nivelAlerta = "MEDIO";
                else
                    nivelAlerta = "BAJO";

                // ✅ NUEVO: Calcular uso por hora para este día específico
                var usoPorHora = new List<UsoHorarioDto>();

                // 🔍 DEBUG: Verificar si hay vuelos con hora
                var vuelosConHoraDelDia = vuelosSalida
         .Where(v => v.Fecha.Date == fecha)
          .Select(v => new { v.NumeroVuelo, Hora = v.HoraSalida, Tipo = "Salida Ida" })
.Concat(vuelosLlegada
       .Where(v => v.Fecha.Date == fecha)
      .Select(v => new { v.NumeroVuelo, Hora = v.HoraLlegada, Tipo = "Llegada Ida" }))
       .Concat(vuelosRegresoComoSalida
  .Where(vr => vr.FechaRegreso.Date == fecha)
      .Select(vr => new { vr.Vuelo.NumeroVuelo, Hora = vr.HoraSalida, Tipo = "Salida Regreso" }))
      .Concat(vuelosRegresoComoLlegada
     .Where(vr => vr.FechaRegreso.Date == fecha)
  .Select(vr => new { vr.Vuelo.NumeroVuelo, Hora = vr.HoraLlegada, Tipo = "Llegada Regreso" }))
    .ToList();

                // Log para depuración (esto aparecerá en la consola del servidor)
                Console.WriteLine($"📅 Día {fecha:yyyy-MM-dd}: {vuelosConHoraDelDia.Count} vuelos encontrados");
                foreach (var v in vuelosConHoraDelDia)
                {
                    Console.WriteLine($"   - Vuelo {v.NumeroVuelo} ({v.Tipo}): Hora {v.Hora}");
                }

                for (int hora = 0; hora < 24; hora++)
                {
                    var horaInicio = new TimeSpan(hora, 0, 0);
                    var horaFin = new TimeSpan(hora, 59, 59);

                    // ✅ Salidas: vuelos normales + regresos
                    var salidasHora = vuelosSalida.Count(v =>
                        v.Fecha.Date == fecha &&
                  v.HoraSalida >= horaInicio &&
                   v.HoraSalida <= horaFin)
                     + vuelosRegresoComoSalida.Count(vr =>
                 vr.FechaRegreso.Date == fecha &&
                       vr.HoraSalida >= horaInicio &&
                    vr.HoraSalida <= horaFin);

                    // ✅ Llegadas: vuelos normales + regresos
                    var llegadasHora = vuelosLlegada.Count(v =>
                 v.Fecha.Date == fecha &&
                v.HoraLlegada >= horaInicio &&
                 v.HoraLlegada <= horaFin)
                 + vuelosRegresoComoLlegada.Count(vr =>
                    vr.FechaRegreso.Date == fecha &&
                    vr.HoraLlegada >= horaInicio &&
                vr.HoraLlegada <= horaFin);

                    var totalHora = salidasHora + llegadasHora;

                    // 🔍 DEBUG: Log de cada hora
                    if (totalHora > 0)
                    {
                        Console.WriteLine($"   ⏰ Hora {hora}: {salidasHora} salidas + {llegadasHora} llegadas = {totalHora} total");
                    }

                    // Solo agregar horas con actividad
                    if (totalHora > 0)
                    {
                        var capacidadHora = aeropuerto.CapacidadVuelosPorHora;
                        var porcentajeHora = capacidadHora > 0
                       ? (decimal)totalHora / capacidadHora * 100
                          : 0;

                        usoPorHora.Add(new UsoHorarioDto
                        {
                            Hora = hora,
                            HoraFormato = $"{hora:D2}:00 - {hora:D2}:59",
                            VuelosSalida = salidasHora,
                            VuelosLlegada = llegadasHora,
                            TotalVuelos = totalHora,
                            CapacidadPorHora = capacidadHora,
                            CapacidadDisponibleHora = Math.Max(0, capacidadHora - totalHora),
                            PorcentajeUsoHora = Math.Round(porcentajeHora, 2),
                            SobreCapacidadHora = totalHora > capacidadHora
                        });
                    }
                }

                Console.WriteLine($"   ✅ Total horas con vuelos: {usoPorHora.Count}");

                diasConVuelos.Add(new UsoDiarioDto
                {
                    Fecha = fecha,
                    FechaFormato = fecha.ToString("ddd, dd MMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                    DiaSemana = (int)fecha.DayOfWeek,
                    NombreDiaSemana = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")),
                    VuelosSalida = salidaDelDia,
                    VuelosLlegada = llegadaDelDia,
                    TotalVuelos = totalDelDia,
                    CapacidadDiaria = capacidadDiaria,
                    CapacidadDisponible = Math.Max(0, capacidadDiaria - totalDelDia),
                    PorcentajeUso = Math.Round(porcentajeUso, 2),
                    SobreCapacidad = sobreCapacidad,
                    NivelAlerta = nivelAlerta,
                    UsoPorHora = usoPorHora // ✅ NUEVO
                });
            }

            var totalSalidas = vuelosSalida.Count + vuelosRegresoComoSalida.Count; // ✅ Incluir regresos
            var totalLlegadas = vuelosLlegada.Count + vuelosRegresoComoLlegada.Count; // ✅ Incluir regresos
            var totalVuelos = totalSalidas + totalLlegadas;
            var diasSobreCapacidad = diasConVuelos.Count(d => d.SobreCapacidad);
            var promedioUso = diasConVuelos.Any()
                  ? diasConVuelos.Average(d => d.PorcentajeUso)
               : 0;

            return new AeropuertoCapacidadDto
            {
                Codigo = aeropuerto.Codigo,
                Nombre = aeropuerto.Nombre,
                Ciudad = aeropuerto.Ciudad,
                Pais = aeropuerto.Pais,
                CapacidadPorHora = aeropuerto.CapacidadVuelosPorHora, // ✅ AGREGADO
                CapacidadDiaria = capacidadDiaria,
                TotalDiasConVuelos = diasConVuelos.Count,
                TotalVuelosSalida = totalSalidas,
                TotalVuelosLlegada = totalLlegadas,
                TotalVuelos = totalVuelos,
                DiasConVuelos = diasConVuelos,
                PorcentajeUsoPromedio = Math.Round(promedioUso, 2),
                DiasSobreCapacidad = diasSobreCapacidad
            };
        }

        public async Task<ReporteCapacidadAeropuertosDto> ObtenerReporteCapacidadTodosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var aeropuertos = await _aeropuertoRepository.ObtenerTodosOrdenadosAsync();
            var reportes = new List<AeropuertoCapacidadDto>();

            foreach (var aeropuerto in aeropuertos)
            {
                var capacidad = await ObtenerCapacidadAeropuertoAsync(aeropuerto.Codigo, fechaInicio, fechaFin);
                reportes.Add(capacidad);
            }

            var aeropuertosSobreCapacidad = reportes.Count(r => r.DiasSobreCapacidad > 0);
            var totalDias = (fechaFin.Date - fechaInicio.Date).Days + 1;

            return new ReporteCapacidadAeropuertosDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalDias = totalDias,
                Aeropuertos = reportes.OrderByDescending(r => r.PorcentajeUsoPromedio).ToList(),
                TotalAeropuertos = reportes.Count,
                AeropuertosSobreCapacidad = aeropuertosSobreCapacidad
            };
        }
    }
}