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

            var vuelosActivos = await _vueloRepository.Context.Vuelos
                .Where(v => v.Matricula == matricula
                    && (v.Estado == "Programado" || v.Estado == "En Curso"))
                .CountAsync();

            if (vuelosActivos > 0)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la aeronave '{matricula}'. " +
                    $"Tiene {vuelosActivos} vuelo(s) programado(s) o en curso. " +
                    $"Debe cancelar o completar todos los vuelos antes de eliminar la aeronave.");
            }

            var asignacionActiva = await _aeronaveRepository.Context.AsignacionesEquipoAeronave
                .FirstOrDefaultAsync(a => a.Matricula == matricula && a.Activa);

            if (asignacionActiva != null)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la aeronave '{matricula}'. " +
                    $"Tiene un equipo de tripulación asignado actualmente. " +
                    $"Debe desasignar el equipo antes de eliminar la aeronave.");
            }

            _aeronaveRepository.Delete(aeronave);
            await _aeronaveRepository.SaveAsync();

            return true;
        }

        public async Task<AeronavesDisponiblesResponseDto> ObtenerAeronavesDisponiblesParaHorarioAsync(
            DateTime fecha,
            TimeSpan horaSalida,
            TimeSpan horaLlegada,
            int? vueloIdExcluir = null)
        {
            var resultado = new AeronavesDisponiblesResponseDto
            {
                Parametros = new ParametrosBusquedaAeronaveDto
                {
                    Fecha = fecha.ToString("yyyy-MM-dd"),
                    HoraSalida = FormatearHora(horaSalida),
                    HoraLlegada = FormatearHora(horaLlegada),
                    VueloIdExcluir = vueloIdExcluir
                }
            };

            // Obtener todas las aeronaves con sus asientos
            var todasAeronaves = await _aeronaveRepository.Context.Aeronaves
                .Include(a => a.Asientos)
                .ToListAsync();

            // Obtener asignaciones de equipos activas
            var asignacionesEquipo = await _aeronaveRepository.Context.AsignacionesEquipoAeronave
                .Include(ae => ae.Equipo)
                .Where(ae => ae.Activa)
                .ToListAsync();

            // Obtener vuelos del día para verificar conflictos
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);
            var vuelosDelDia = await _vueloRepository.Context.Vuelos
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Where(v => v.Fecha >= fechaInicio && v.Fecha < fechaFin && v.Estado != "Cancelado")
                .ToListAsync();

            int enMantenimiento = 0;
            int sinEquipo = 0;
            int conConflicto = 0;

            foreach (var aeronave in todasAeronaves)
            {
                var asignacion = asignacionesEquipo.FirstOrDefault(ae => ae.Matricula == aeronave.Matricula);
                var vuelosAeronave = vuelosDelDia.Where(v => v.Matricula == aeronave.Matricula).ToList();

                // Verificar si está en mantenimiento
                if (aeronave.Estado != "Operativa")
                {
                    enMantenimiento++;
                    resultado.NoDisponibles.Add(new AeronaveNoDisponibleDto
                    {
                        Matricula = aeronave.Matricula,
                        Modelo = aeronave.Modelo,
                        Capacidad = aeronave.Capacidad,
                        Estado = aeronave.Estado,
                        Razon = $"Aeronave en estado '{aeronave.Estado}'. Solo se permiten aeronaves operativas.",
                        CodigoRazon = "NO_OPERATIVA"
                    });
                    continue;
                }

                // Verificar si tiene equipo asignado
                if (asignacion == null)
                {
                    sinEquipo++;
                    resultado.NoDisponibles.Add(new AeronaveNoDisponibleDto
                    {
                        Matricula = aeronave.Matricula,
                        Modelo = aeronave.Modelo,
                        Capacidad = aeronave.Capacidad,
                        Estado = aeronave.Estado,
                        Razon = "No tiene equipo de tripulación asignado. Debe asignar un equipo antes de programar vuelos.",
                        CodigoRazon = "SIN_EQUIPO"
                    });
                    continue;
                }

                // Verificar conflictos de horario
                var conflicto = VerificarConflictoHorario(aeronave, vuelosAeronave, horaSalida, horaLlegada, vueloIdExcluir);
                if (conflicto != null)
                {
                    conConflicto++;
                    var vueloConflicto = conflicto.Item1;
                    var disponibleDesde = conflicto.Item2;

                    resultado.NoDisponibles.Add(new AeronaveNoDisponibleDto
                    {
                        Matricula = aeronave.Matricula,
                        Modelo = aeronave.Modelo,
                        Capacidad = aeronave.Capacidad,
                        Estado = aeronave.Estado,
                        Razon = $"Tiene un vuelo programado ({vueloConflicto.NumeroVuelo}) que se solapa con el horario solicitado.",
                        CodigoRazon = "CONFLICTO_HORARIO",
                        VueloConflicto = new VueloConflictoDto
                        {
                            Id = vueloConflicto.Id,
                            NumeroVuelo = vueloConflicto.NumeroVuelo ?? "N/A",
                            HoraSalida = FormatearHora(vueloConflicto.HoraSalida),
                            HoraLlegada = FormatearHora(vueloConflicto.HoraLlegada),
                            Ruta = $"{vueloConflicto.OrigenCodigo} → {vueloConflicto.DestinoCodigo}",
                            Estado = vueloConflicto.Estado ?? "Programado"
                        },
                        DisponibleDesde = disponibleDesde != null ? FormatearHora(disponibleDesde.Value) : null
                    });
                    continue;
                }

                // ✅ Aeronave disponible
                var proximoVuelo = vuelosAeronave
                    .Where(v => v.HoraSalida > horaLlegada)
                    .OrderBy(v => v.HoraSalida)
                    .FirstOrDefault();

                resultado.Disponibles.Add(new AeronaveDisponibleDto
                {
                    Matricula = aeronave.Matricula,
                    Modelo = aeronave.Modelo,
                    Capacidad = aeronave.Capacidad,
                    Estado = aeronave.Estado,
                    TiempoPreparacionMinutos = aeronave.TiempoPreparacionMinutos,
                    EquipoAsignado = asignacion.Equipo != null ? new EquipoAsignadoInfoDto
                    {
                        Id = asignacion.Equipo.Id,
                        Nombre = asignacion.Equipo.Nombre ?? "N/A",
                        Codigo = asignacion.Equipo.Codigo ?? "N/A",
                        Estado = asignacion.Equipo.Estado ?? "Disponible"
                    } : null,
                    ProximoVuelo = proximoVuelo != null ? new ProximoVueloDto
                    {
                        Id = proximoVuelo.Id,
                        NumeroVuelo = proximoVuelo.NumeroVuelo ?? "N/A",
                        HoraSalida = FormatearHora(proximoVuelo.HoraSalida),
                        HoraLlegada = FormatearHora(proximoVuelo.HoraLlegada),
                        Ruta = $"{proximoVuelo.OrigenCodigo} → {proximoVuelo.DestinoCodigo}"
                    } : null,
                    VuelosDelDia = vuelosAeronave.Count,
                    AsientosPorClase = new AsientosPorClaseDto
                    {
                        Primera = aeronave.Asientos?.Count(a => a.Clase == "Primera") ?? 0,
                        Ejecutiva = aeronave.Asientos?.Count(a => a.Clase == "Ejecutiva") ?? 0,
                        Economica = aeronave.Asientos?.Count(a => a.Clase == "Economica") ?? 0,
                        Total = aeronave.Asientos?.Count ?? 0
                    }
                });
            }

            // Ordenar disponibles por menor cantidad de vuelos del día (priorizar las menos ocupadas)
            resultado.Disponibles = resultado.Disponibles
                .OrderBy(a => a.VuelosDelDia)
                .ThenBy(a => a.Matricula)
                .ToList();

            // Resumen
            resultado.Resumen = new ResumenDisponibilidadDto
            {
                TotalAeronaves = todasAeronaves.Count,
                Disponibles = resultado.Disponibles.Count,
                NoDisponibles = resultado.NoDisponibles.Count,
                EnMantenimiento = enMantenimiento,
                SinEquipo = sinEquipo,
                ConConflictoHorario = conConflicto
            };

            return resultado;
        }

        /// <summary>
        /// Verifica si hay conflicto de horario con vuelos existentes
        /// Considera vuelos que cruzan la medianoche
        /// </summary>
        private Tuple<Vuelo, TimeSpan?>? VerificarConflictoHorario(
            Aeronave aeronave, 
            List<Vuelo> vuelosAeronave, 
            TimeSpan horaSalida, 
            TimeSpan horaLlegada,
            int? vueloIdExcluir)
        {
            int tiempoPreparacion = aeronave.TiempoPreparacionMinutos > 0 
                ? aeronave.TiempoPreparacionMinutos 
                : 120;

            // Determinar si el vuelo solicitado cruza la medianoche
            bool vueloSolicitadoCruzaMedianoche = horaLlegada < horaSalida;

            foreach (var vuelo in vuelosAeronave)
            {
                // Excluir el vuelo que se está editando
                if (vueloIdExcluir.HasValue && vuelo.Id == vueloIdExcluir.Value)
                    continue;

                // Determinar si el vuelo existente cruza la medianoche
                bool vueloExistenteCruzaMedianoche = vuelo.HoraLlegada < vuelo.HoraSalida;

                // Calcular rangos con tiempo de preparación
                var vueloInicioConMargen = vuelo.HoraSalida.Subtract(TimeSpan.FromMinutes(tiempoPreparacion));
                var vueloFinConMargen = vuelo.HoraLlegada.Add(TimeSpan.FromMinutes(tiempoPreparacion));

                // Ajustar para tiempos negativos
                if (vueloInicioConMargen < TimeSpan.Zero)
                    vueloInicioConMargen = vueloInicioConMargen.Add(TimeSpan.FromHours(24));

                // Ajustar para tiempos que pasan de 24 horas
                if (vueloFinConMargen >= TimeSpan.FromHours(24))
                    vueloFinConMargen = vueloFinConMargen.Subtract(TimeSpan.FromHours(24));

                bool haySolapamiento = false;

                if (!vueloSolicitadoCruzaMedianoche && !vueloExistenteCruzaMedianoche)
                {
                    // Caso simple: ninguno cruza la medianoche
                    haySolapamiento = !(horaLlegada <= vueloInicioConMargen || horaSalida >= vueloFinConMargen);
                }
                else
                {
                    // Caso complejo: al menos uno cruza la medianoche
                    // Convertir a minutos desde medianoche para comparar más fácilmente
                    var salidaSolicitadaMin = horaSalida.TotalMinutes;
                    var llegadaSolicitadaMin = horaLlegada.TotalMinutes + (vueloSolicitadoCruzaMedianoche ? 1440 : 0);
                    
                    var salidaExistenteMin = vuelo.HoraSalida.TotalMinutes - tiempoPreparacion;
                    var llegadaExistenteMin = vuelo.HoraLlegada.TotalMinutes + tiempoPreparacion + (vueloExistenteCruzaMedianoche ? 1440 : 0);

                    // Verificar solapamiento
                    haySolapamiento = !(llegadaSolicitadaMin <= salidaExistenteMin || salidaSolicitadaMin >= llegadaExistenteMin);
                }

                if (haySolapamiento)
                {
                    // Calcular cuándo estará disponible
                    var disponibleDesde = vueloFinConMargen;
                    return Tuple.Create(vuelo, (TimeSpan?)disponibleDesde);
                }
            }

            return null;
        }

        private static string FormatearHora(TimeSpan hora)
        {
            var hh = hora.Hours;
            var mm = hora.Minutes;
            var periodo = hh >= 12 ? "PM" : "AM";
            var displayHour = hh == 0 ? 12 : (hh > 12 ? hh - 12 : hh);
            return $"{displayHour}:{mm:D2} {periodo}";
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
            var aeronaveConAsientos = await _aeronaveRepository.Context.Aeronaves
                .Include(a => a.Asientos)
                .FirstOrDefaultAsync(a => a.Matricula == aeronave.Matricula);

            if (aeronaveConAsientos == null)
            {
                throw new KeyNotFoundException($"Aeronave {aeronave.Matricula} no encontrada");
            }

            var todosVuelos = await _vueloRepository.GetAllAsync();
            var vuelosAeronave = todosVuelos.Where(v => v.Matricula == aeronave.Matricula
                 && v.Estado != "Cancelado").ToList();

            var vuelosDetallados = new List<Vuelo>();
            foreach (var vuelo in vuelosAeronave)
            {
                var vueloDetalle = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);
                if (vueloDetalle != null)
                {
                    vuelosDetallados.Add(vueloDetalle);
                }
            }

            var asientos = aeronaveConAsientos.Asientos?.ToList() ?? new List<Asiento>();

            var primeraTotal = asientos.Count(a => a.Clase == "Primera");
            var ejecutivaTotal = asientos.Count(a => a.Clase == "Ejecutiva");
            var economicaTotal = asientos.Count(a => a.Clase == "Economica");

            var todasReservas = vuelosDetallados
                .SelectMany(v => v.Reservas ?? Enumerable.Empty<Reserva>())
                .Where(r => r.Estado == "Confirmada")
                .ToList();

            var primeraReservados = todasReservas.Count(r => r.Clase == "Primera");
            var ejecutivaReservados = todasReservas.Count(r => r.Clase == "Ejecutiva");
            var economicaReservados = todasReservas.Count(r => r.Clase == "Economica");

            var primeraDisponibles = primeraTotal - primeraReservados;
            var ejecutivaDisponibles = ejecutivaTotal - ejecutivaReservados;
            var economicaDisponibles = economicaTotal - economicaReservados;

            var totalAsientos = asientos.Count;
            var totalReservados = todasReservas.Count;
            var totalDisponibles = totalAsientos - totalReservados;

            decimal primeraPorcentaje = primeraTotal > 0 ? (decimal)primeraReservados / primeraTotal * 100 : 0;
            decimal ejecutivaPorcentaje = ejecutivaTotal > 0 ? (decimal)ejecutivaReservados / ejecutivaTotal * 100 : 0;
            decimal economicaPorcentaje = economicaTotal > 0 ? (decimal)economicaReservados / economicaTotal * 100 : 0;
            decimal totalPorcentaje = totalAsientos > 0 ? (decimal)totalReservados / totalAsientos * 100 : 0;

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