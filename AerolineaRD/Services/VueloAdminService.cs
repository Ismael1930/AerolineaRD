using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Services
{
    public class VueloAdminService : IVueloAdminService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IEstadoVueloRepository _estadoVueloRepository;
        private readonly ITripulacionRepository _tripulacionRepository;
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly IEmailService _emailService; // ✅ NUEVO
        private readonly IMapper _mapper;

        public VueloAdminService(
            IVueloRepository vueloRepository,
            IEstadoVueloRepository estadoVueloRepository,
            ITripulacionRepository tripulacionRepository,
            IAeronaveRepository aeronaveRepository,
            IEquipoRepository equipoRepository,
            IEmailService emailService, // ✅ NUEVO
            IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _estadoVueloRepository = estadoVueloRepository;
            _tripulacionRepository = tripulacionRepository;
            _aeronaveRepository = aeronaveRepository;
            _equipoRepository = equipoRepository;
            _emailService = emailService; // ✅ NUEVO
            _mapper = mapper;
        }

        public async Task<OperationResult<VueloDetalleDto>> CrearVueloAsync(CrearVueloDto dto)
        {
            var errores = new List<ValidationError>();

            // ✅ VALIDACIÓN 0: Validar que la fecha no sea del pasado
            if (dto.Fecha.Date < DateTime.Today)
            {
                errores.Add(ValidationError.Create(
                    campo: "Fecha",
                    tipo: ValidationErrorType.FechaInvalida,
                    mensaje: $"No se pueden programar vuelos en fechas pasadas. La fecha {dto.Fecha:dd/MM/yyyy} ya pasó.",
                    detalles: new { fechaSolicitada = dto.Fecha, fechaActual = DateTime.Today }
                ));
            }

            // ✅ VALIDACIÓN 0.1: Validar duración del vuelo (máximo 18 horas = 1080 minutos)
            const int DURACION_MAXIMA_MINUTOS = 1080; // 18 horas (vuelos ultra-largos como Singapore-NY)
            const int DURACION_MINIMA_MINUTOS = 15;   // Mínimo 15 minutos para vuelos muy cortos

            // Calcular duración basada en horas de salida/llegada
            int duracionCalculada;
            bool cruzaMedianoche = dto.HoraLlegada < dto.HoraSalida;

            if (cruzaMedianoche)
            {
                // Vuelo nocturno: llegada al día siguiente
                duracionCalculada = (int)(TimeSpan.FromHours(24) - dto.HoraSalida + dto.HoraLlegada).TotalMinutes;
            }
            else
            {
                duracionCalculada = (int)(dto.HoraLlegada - dto.HoraSalida).TotalMinutes;
            }

            if (duracionCalculada < DURACION_MINIMA_MINUTOS)
            {
                errores.Add(ValidationError.Create(
                    campo: "HoraLlegada",
                    tipo: ValidationErrorType.DuracionInvalida,
                    mensaje: $"La duración del vuelo ({duracionCalculada} minutos) es muy corta. " +
                             $"El mínimo permitido es {DURACION_MINIMA_MINUTOS} minutos.",
                    detalles: new
                    {
                        duracionCalculada,
                        duracionMinima = DURACION_MINIMA_MINUTOS,
                        horaSalida = dto.HoraSalida,
                        horaLlegada = dto.HoraLlegada
                    }
                ));
            }
            else if (duracionCalculada > DURACION_MAXIMA_MINUTOS)
            {
                errores.Add(ValidationError.Create(
                    campo: "HoraLlegada",
                    tipo: ValidationErrorType.DuracionInvalida,
                    mensaje: $"La duración del vuelo ({duracionCalculada} minutos ≈ {duracionCalculada / 60}h {duracionCalculada % 60}m) excede el máximo permitido. " +
                             $"Ningún vuelo comercial dura más de {DURACION_MAXIMA_MINUTOS / 60} horas. " +
                             $"Verifique las horas de salida ({dto.HoraSalida:hh\\:mm}) y llegada ({dto.HoraLlegada:hh\\:mm}).",
                    detalles: new
                    {
                        duracionCalculada,
                        duracionMaxima = DURACION_MAXIMA_MINUTOS,
                        horaSalida = dto.HoraSalida,
                        horaLlegada = dto.HoraLlegada,
                        cruzaMedianoche
                    }
                ));
            }

            // Si hay errores de fecha/duración, no continuar
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ✅ VALIDACIÓN 1: Verificar que la aeronave existe, está operativa y tiene equipo asignado
            if (!string.IsNullOrEmpty(dto.Matricula))
            {
                var aeronave = await _aeronaveRepository.GetByIdAsync(dto.Matricula);

                if (aeronave == null)
                {
                    errores.Add(ValidationError.Create(
                        campo: "Matricula",
                        tipo: ValidationErrorType.EntidadNoEncontrada,
                        mensaje: $"Aeronave con matrícula '{dto.Matricula}' no encontrada.",
                        detalles: new { matricula = dto.Matricula }
                    ));
                }
                else
                {
                    // ✅ Validar estado operativo
                    if (aeronave.Estado != "Operativa")
                    {
                        errores.Add(ValidationError.Create(
                            campo: "Matricula",
                            tipo: ValidationErrorType.AeronaveNoOperativa,
                            mensaje: $"La aeronave '{dto.Matricula}' no está operativa. Estado actual: {aeronave.Estado}. " +
                                     $"Solo se pueden programar vuelos con aeronaves en estado 'Operativa'.",
                            detalles: new
                            {
                                matricula = dto.Matricula,
                                estadoActual = aeronave.Estado,
                                estadosPermitidos = new[] { "Operativa" }
                            }
                        ));
                    }

                    // ✅ Validar que tenga equipo asignado
                    var asignacionEquipo = await _equipoRepository.Context.AsignacionesEquipoAeronave
                        .Include(ae => ae.Equipo)
                        .FirstOrDefaultAsync(ae => ae.Matricula == dto.Matricula && ae.Activa);

                    if (asignacionEquipo == null)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "Matricula",
                            tipo: ValidationErrorType.AeronaveSinEquipo,
                            mensaje: $"La aeronave '{dto.Matricula}' no tiene ningún equipo de tripulación asignado. " +
                                     $"Debe asignar un equipo antes de programar vuelos con esta aeronave.",
                            detalles: new
                            {
                                matricula = dto.Matricula,
                                equipoAsignado = false,
                                accionRequerida = "Asignar equipo en el módulo de tripulación"
                            }
                        ));
                    }
                    else
                    {
                        // ✅ Validar disponibilidad del equipo
                        var equipo = asignacionEquipo.Equipo;
                        if (equipo != null)
                        {
                            if (equipo.Estado == "En Servicio")
                            {
                                errores.Add(ValidationError.Create(
                                    campo: "Matricula",
                                    tipo: ValidationErrorType.EquipoNoDisponible,
                                    mensaje: $"El equipo '{equipo.Nombre}' asignado a la aeronave '{dto.Matricula}' " +
                                             $"está actualmente en servicio en otro vuelo.",
                                    detalles: new
                                    {
                                        matricula = dto.Matricula,
                                        equipoId = equipo.Id,
                                        equipoNombre = equipo.Nombre,
                                        estadoEquipo = equipo.Estado
                                    }
                                ));
                            }
                            else if (equipo.Estado == "Descanso" && equipo.DisponibleDesde.HasValue)
                            {
                                var fechaHoraSolicitada = dto.Fecha.Date.Add(dto.HoraSalida);
                                if (fechaHoraSolicitada < equipo.DisponibleDesde.Value)
                                {
                                    errores.Add(ValidationError.Create(
                                        campo: "Matricula",
                                        tipo: ValidationErrorType.EquipoNoDisponible,
                                        mensaje: $"El equipo '{equipo.Nombre}' asignado a la aeronave '{dto.Matricula}' " +
                                                 $"está en período de descanso hasta el {equipo.DisponibleDesde.Value:dd/MM/yyyy HH:mm}. " +
                                                 $"No puede programar vuelos antes de esa fecha/hora.",
                                        detalles: new
                                        {
                                            matricula = dto.Matricula,
                                            equipoId = equipo.Id,
                                            equipoNombre = equipo.Nombre,
                                            estadoEquipo = equipo.Estado,
                                            disponibleDesde = equipo.DisponibleDesde.Value,
                                            fechaVueloSolicitada = fechaHoraSolicitada
                                        }
                                    ));
                                }
                            }
                        }
                    }
                }
            }

            // Si hay errores críticos de aeronave/equipo, no continuar
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ? 2. VALIDACIÓN: Verificar disponibilidad de aeronave (horarios)
            if (!string.IsNullOrEmpty(dto.Matricula))
            {
                bool aeronaveDisponible = await _vueloRepository.EstaAeronaveDisponibleAsync(
                    dto.Matricula,
                    dto.Fecha,
                    dto.HoraSalida,
                    dto.HoraLlegada);

                if (!aeronaveDisponible)
                {
                    errores.Add(ValidationError.Create(
                        campo: "Matricula",
                        tipo: ValidationErrorType.AeronaveNoDisponible,
                        mensaje: $"La aeronave con matrícula '{dto.Matricula}' no está disponible en el horario especificado. " +
                                 $"Ya tiene un vuelo asignado que se solapa con el horario {dto.HoraSalida:hh\\:mm} - {dto.HoraLlegada:hh\\:mm} el {dto.Fecha:dd/MM/yyyy}. " +
                                 $"Recuerde que se requiere un tiempo mínimo de preparación entre vuelos.",
                        detalles: new
                        {
                            matricula = dto.Matricula,
                            fecha = dto.Fecha,
                            horaSalida = dto.HoraSalida,
                            horaLlegada = dto.HoraLlegada
                        }
                    ));
                }
            }

            // ? 2. VALIDACIÓN: Verificar capacidad del aeropuerto de origen
            if (!string.IsNullOrEmpty(dto.OrigenCodigo))
            {
                bool origenTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                    dto.OrigenCodigo,
                    dto.Fecha,
                    dto.HoraSalida,
                    true);

                if (!origenTieneCapacidad)
                {
                    errores.Add(ValidationError.Create(
                        campo: "OrigenCodigo",
                        tipo: ValidationErrorType.AeropuertoSinCapacidad,
                        mensaje: $"El aeropuerto de origen '{dto.OrigenCodigo}' ha alcanzado su capacidad máxima de despegues " +
                                 $"en el horario {dto.HoraSalida:hh\\:mm} el {dto.Fecha:dd/MM/yyyy}. " +
                                 $"Por favor, seleccione otro horario.",
                        detalles: new
                        {
                            codigoAeropuerto = dto.OrigenCodigo,
                            fecha = dto.Fecha,
                            horario = dto.HoraSalida,
                            tipoOperacion = "salida"
                        }
                    ));
                }
            }

            // ? 3. VALIDACIÓN: Verificar capacidad del aeropuerto de destino
            if (!string.IsNullOrEmpty(dto.DestinoCodigo))
            {
                bool destinoTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                    dto.DestinoCodigo,
                    dto.Fecha,
                    dto.HoraLlegada,
                    false);

                if (!destinoTieneCapacidad)
                {
                    errores.Add(ValidationError.Create(
                        campo: "DestinoCodigo",
                        tipo: ValidationErrorType.AeropuertoSinCapacidad,
                        mensaje: $"El aeropuerto de destino '{dto.DestinoCodigo}' ha alcanzado su capacidad máxima de aterrizajes " +
                                 $"en el horario {dto.HoraLlegada:hh\\:mm} el {dto.Fecha:dd/MM/yyyy}. " +
                                 $"Por favor, seleccione otro horario.",
                        detalles: new
                        {
                            codigoAeropuerto = dto.DestinoCodigo,
                            fecha = dto.Fecha,
                            horario = dto.HoraLlegada,
                            tipoOperacion = "llegada"
                        }
                    ));
                }
            }

            // ? Si hay errores hasta ahora, no continuar
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ? Si hay errores de validación, devolver resultado fallido
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ? Todo válido, crear el vuelo
            var vuelo = _mapper.Map<Vuelo>(dto);
            vuelo.Estado = "Programado";

            // ✅ NUEVO: Siempre establecer las 3 clases disponibles
            vuelo.ClasesDisponibles = "Economica,Ejecutiva,Primera";

            // ? Asignar clase del vuelo (por defecto Económica, pero el vuelo soporta las 3 clases)
            if (!string.IsNullOrEmpty(dto.Clase))
            {
                vuelo.Clase = dto.Clase;
            }

            // ✅ NUEVO: Asignar TipoVuelo y FechaRegreso
            if (!string.IsNullOrEmpty(dto.TipoVuelo))
            {
                vuelo.TipoVuelo = dto.TipoVuelo;
            }

            // Solo asignar FechaRegreso si el vuelo es de IdaYVuelta
            if (dto.TipoVuelo == "IdaYVuelta" && dto.FechaRegreso.HasValue)
            {
                vuelo.FechaRegreso = dto.FechaRegreso.Value;
            }
            else
            {
                vuelo.FechaRegreso = null; // Asegurar que sea null para vuelos de SoloIda
            }

            // ? 4. VALIDACIÓN: Asignar y validar tripulación
            if (dto.IdsTripulacion != null && dto.IdsTripulacion.Any())
            {
                // ✅ Reutilizar la aeronave ya consultada arriba
                foreach (var idTripulacion in dto.IdsTripulacion)
                {
                    var tripulacion = await _tripulacionRepository.GetByIdAsync(idTripulacion);
                    if (tripulacion == null)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "IdsTripulacion",
                            tipo: ValidationErrorType.EntidadNoEncontrada,
                            mensaje: $"Tripulante con ID {idTripulacion} no encontrado.",
                            detalles: new { idTripulacion }
                        ));
                        continue;
                    }

                    // Verificar disponibilidad del tripulante
                    bool tripulanteDisponible = await _tripulacionRepository.EstaTripulacionDisponibleAsync(
                        idTripulacion,
                        dto.Fecha,
                        dto.HoraSalida,
                        dto.HoraLlegada);

                    if (!tripulanteDisponible)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "IdsTripulacion",
                            tipo: ValidationErrorType.TripulanteNoDisponible,
                            mensaje: $"El tripulante {tripulacion.Nombre} {tripulacion.Apellido} no está disponible en el horario especificado. " +
                                     $"Ya tiene un vuelo asignado o no ha cumplido el tiempo mínimo de descanso (8 horas).",
                            detalles: new
                            {
                                idTripulacion,
                                nombre = $"{tripulacion.Nombre} {tripulacion.Apellido}",
                                rol = tripulacion.Rol,
                                fecha = dto.Fecha,
                                horaSalida = dto.HoraSalida,
                                horaLlegada = dto.HoraLlegada
                            }
                        ));
                        continue;
                    }

                    // ❌ DESHABILITADO: Validación de certificación (se implementará más adelante)
                    // TODO: Implementar validación de certificaciones de tripulación para pilotos y copilotos

                    vuelo.Tripulaciones.Add(tripulacion);
                }
            }

            // ? Si hay errores de validación, devolver resultado fallido
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ? Todo válido, crear el vuelo
            await _vueloRepository.AddAsync(vuelo);
            await _vueloRepository.SaveAsync();

            // Crear estado inicial del vuelo
            var estadoVuelo = new EstadoVuelo
            {
                IdVuelo = vuelo.Id,
                Estado = "Programado",
                HoraSalidaProgramada = dto.Fecha.Date.Add(dto.HoraSalida),
                HoraLlegadaProgramada = dto.Fecha.Date.Add(dto.HoraLlegada)
            };

            await _estadoVueloRepository.AddAsync(estadoVuelo);
            await _estadoVueloRepository.SaveAsync();

            var vueloCreado = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);
            var vueloDto = _mapper.Map<VueloDetalleDto>(vueloCreado);

            // ✅ NUEVO: Agregar información de las 3 clases disponibles con sus precios
            vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto>
 {
    new ClaseDisponibilidadDto
{
   Clase = "Economica",
 AsientosDisponibles = vueloCreado?.Aeronave?.Asientos?.Count(a => a.Clase == "Economica") ?? 0,
Precio = dto.PrecioBase
            },
 new ClaseDisponibilidadDto
    {
   Clase = "Ejecutiva",
   AsientosDisponibles = vueloCreado?.Aeronave?.Asientos?.Count(a => a.Clase == "Ejecutiva") ?? 0,
       Precio = dto.PrecioBase + 100m
   },
     new ClaseDisponibilidadDto
{
  Clase = "Primera",
 AsientosDisponibles = vueloCreado?.Aeronave?.Asientos?.Count(a => a.Clase == "Primera") ?? 0,
Precio = dto.PrecioBase + 200m
   }
    };

            return OperationResult<VueloDetalleDto>.SuccessResult(
               vueloDto,
               $"Vuelo creado exitosamente. Clases disponibles: Económica (${dto.PrecioBase}), Ejecutiva (${dto.PrecioBase + 100m}), Primera (${dto.PrecioBase + 200m})"
                 );
        }

        public async Task<VueloDetalleDto?> ObtenerVueloDetalleAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);

            if (vuelo == null)
                return null;

            var vueloDto = _mapper.Map<VueloDetalleDto>(vuelo);

            // ✅ Agregar información de las 3 clases disponibles con sus precios
            vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto>
   {
         new ClaseDisponibilidadDto
                {
   Clase = "Economica",
           AsientosDisponibles = vuelo.Aeronave?.Asientos?.Count(a => a.Clase == "Economica") ?? 0,
  Precio = vuelo.PrecioBase
      },
      new ClaseDisponibilidadDto
    {
         Clase = "Ejecutiva",
        AsientosDisponibles = vuelo.Aeronave?.Asientos?.Count(a => a.Clase == "Ejecutiva") ?? 0,
          Precio = vuelo.PrecioBase + 100m
            },
     new ClaseDisponibilidadDto
       {
       Clase = "Primera",
        AsientosDisponibles = vuelo.Aeronave?.Asientos?.Count(a => a.Clase == "Primera") ?? 0,
Precio = vuelo.PrecioBase + 200m
 }
   };

            // ? Agregar información de la aeronave si existe
            if (vuelo.Aeronave != null)
            {
                vueloDto.Aeronave = new AeronaveInfoDto
                {
                    Matricula = vuelo.Aeronave.Matricula,
                    Modelo = vuelo.Aeronave.Modelo,
                    Capacidad = vuelo.Aeronave.Capacidad,
                    Estado = vuelo.Aeronave.Estado,
                    TiempoPreparacionMinutos = vuelo.Aeronave.TiempoPreparacionMinutos,
                    TotalAsientos = vuelo.Aeronave.Asientos?.Count ?? 0
                };
            }

            return vueloDto;
        }

        public async Task<OperationResult<VueloDetalleDto>> ActualizarVueloAsync(ActualizarVueloDto dto)
        {
            var errores = new List<ValidationError>();

            var vuelo = await _vueloRepository.GetByIdAsync(dto.Id);
            if (vuelo == null)
            {
                errores.Add(ValidationError.Create(
                    campo: "Id",
                   tipo: ValidationErrorType.EntidadNoEncontrada,
          mensaje: $"Vuelo con ID {dto.Id} no encontrado.",
         detalles: new { id = dto.Id }
                       ));
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // ✅ NUEVO: Guardar valores anteriores para notificaciones
            var fechaAnterior = vuelo.Fecha;
            var horaSalidaAnterior = vuelo.HoraSalida;
            var horaLlegadaAnterior = vuelo.HoraLlegada;
            var estadoAnterior = vuelo.Estado;
            var origenAnterior = vuelo.OrigenCodigo;
            var destinoAnterior = vuelo.DestinoCodigo;

            // ✅ Validación: Si se está cambiando la aeronave o el horario, verificar disponibilidad
            bool cambioAeronave = !string.IsNullOrEmpty(dto.Matricula) && dto.Matricula != vuelo.Matricula;
            bool cambioFecha = dto.Fecha.HasValue && dto.Fecha.Value.Date != vuelo.Fecha.Date;
            bool cambioHoraSalida = dto.HoraSalida.HasValue && dto.HoraSalida.Value != vuelo.HoraSalida;
            bool cambioHoraLlegada = dto.HoraLlegada.HasValue && dto.HoraLlegada.Value != vuelo.HoraLlegada;
            bool cambioEstado = !string.IsNullOrEmpty(dto.Estado) && dto.Estado != vuelo.Estado;

            if (cambioAeronave || cambioFecha || cambioHoraSalida || cambioHoraLlegada)
            {
                string matriculaValidar = dto.Matricula ?? vuelo.Matricula ?? "";
                DateTime fechaValidar = dto.Fecha ?? vuelo.Fecha;
                TimeSpan horaSalidaValidar = dto.HoraSalida ?? vuelo.HoraSalida;
                TimeSpan horaLlegadaValidar = dto.HoraLlegada ?? vuelo.HoraLlegada;

                if (!string.IsNullOrEmpty(matriculaValidar))
                {
                    bool aeronaveDisponible = await _vueloRepository.EstaAeronaveDisponibleAsync(
                        matriculaValidar,
                        fechaValidar,
                        horaSalidaValidar,
                        horaLlegadaValidar,
                        dto.Id);

                    if (!aeronaveDisponible)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "Matricula",
                            tipo: ValidationErrorType.AeronaveNoDisponible,
                            mensaje: $"La aeronave con matrícula '{matriculaValidar}' no está disponible en el horario especificado.",
                            detalles: new { matricula = matriculaValidar, fecha = fechaValidar }
                        ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.OrigenCodigo))
                {
                    bool origenTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                        dto.OrigenCodigo, fechaValidar, horaSalidaValidar, true);

                    if (!origenTieneCapacidad)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "OrigenCodigo",
                            tipo: ValidationErrorType.AeropuertoSinCapacidad,
                            mensaje: $"El aeropuerto de origen ha alcanzado su capacidad máxima.",
                            detalles: new { codigoAeropuerto = dto.OrigenCodigo }
                        ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.DestinoCodigo))
                {
                    bool destinoTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                        dto.DestinoCodigo, fechaValidar, horaLlegadaValidar, false);

                    if (!destinoTieneCapacidad)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "DestinoCodigo",
                            tipo: ValidationErrorType.AeropuertoSinCapacidad,
                            mensaje: $"El aeropuerto de destino ha alcanzado su capacidad máxima.",
                            detalles: new { codigoAeropuerto = dto.DestinoCodigo }
                        ));
                    }
                }
            }

            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // Aplicar cambios
            if (!string.IsNullOrEmpty(dto.NumeroVuelo)) vuelo.NumeroVuelo = dto.NumeroVuelo;
            if (dto.Fecha.HasValue) vuelo.Fecha = dto.Fecha.Value;
            if (dto.HoraSalida.HasValue) vuelo.HoraSalida = dto.HoraSalida.Value;
            if (dto.HoraLlegada.HasValue) vuelo.HoraLlegada = dto.HoraLlegada.Value;
            if (dto.Duracion.HasValue) vuelo.Duracion = dto.Duracion.Value;
            if (dto.PrecioBase.HasValue) vuelo.PrecioBase = dto.PrecioBase.Value;
            if (!string.IsNullOrEmpty(dto.OrigenCodigo)) vuelo.OrigenCodigo = dto.OrigenCodigo;
            if (!string.IsNullOrEmpty(dto.DestinoCodigo)) vuelo.DestinoCodigo = dto.DestinoCodigo;
            if (!string.IsNullOrEmpty(dto.Matricula)) vuelo.Matricula = dto.Matricula;
            if (!string.IsNullOrEmpty(dto.Estado)) vuelo.Estado = dto.Estado;
            if (!string.IsNullOrEmpty(dto.TipoVuelo)) vuelo.TipoVuelo = dto.TipoVuelo;
            if (!string.IsNullOrEmpty(dto.Clase)) vuelo.Clase = dto.Clase;

            if (dto.FechaRegreso.HasValue)
            {
                vuelo.FechaRegreso = dto.FechaRegreso.Value;
            }
            else if (dto.TipoVuelo == "SoloIda")
            {
                vuelo.FechaRegreso = null;
            }

            _vueloRepository.Update(vuelo);
            await _vueloRepository.SaveAsync();

            // ✅ NUEVO: Enviar notificaciones si hubo cambios relevantes
            string mensajeNotificacion = "";
            if (cambioFecha || cambioHoraSalida || cambioHoraLlegada || cambioEstado)
            {
                var tipoCambio = DeterminarTipoCambio(
                    fechaAnterior, vuelo.Fecha,
                    horaSalidaAnterior, vuelo.HoraSalida,
                    estadoAnterior, vuelo.Estado);

                var cambioInfo = new CambioVueloInfo
                {
                    NumeroVuelo = vuelo.NumeroVuelo ?? $"Vuelo #{vuelo.Id}",
                    Origen = vuelo.OrigenCodigo ?? origenAnterior ?? "",
                    Destino = vuelo.DestinoCodigo ?? destinoAnterior ?? "",
                    FechaAnterior = cambioFecha ? fechaAnterior : null,
                    FechaNueva = cambioFecha ? vuelo.Fecha : null,
                    HoraSalidaAnterior = cambioHoraSalida ? horaSalidaAnterior : null,
                    HoraSalidaNueva = cambioHoraSalida ? vuelo.HoraSalida : null,
                    HoraLlegadaAnterior = cambioHoraLlegada ? horaLlegadaAnterior : null,
                    HoraLlegadaNueva = cambioHoraLlegada ? vuelo.HoraLlegada : null,
                    EstadoAnterior = cambioEstado ? estadoAnterior : null,
                    EstadoNuevo = cambioEstado ? vuelo.Estado : null,
                    TipoCambio = tipoCambio,
                    MensajeAdicional = dto.MensajeNotificacion
                };

                var resultadoNotificacion = await _emailService.NotificarCambioVueloAsync(dto.Id, cambioInfo);
                mensajeNotificacion = resultadoNotificacion.Exitoso 
                    ? $" {resultadoNotificacion.Mensaje}"
                    : $" (Notificaciones: {resultadoNotificacion.Mensaje})";
            }

            var vueloActualizado = await _vueloRepository.ObtenerVueloConDetallesAsync(dto.Id);
            var vueloDto = _mapper.Map<VueloDetalleDto>(vueloActualizado);

            // Agregar información de clases disponibles
            vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto>
            {
                new ClaseDisponibilidadDto
                {
                    Clase = "Economica",
                    AsientosDisponibles = vueloActualizado?.Aeronave?.Asientos?.Count(a => a.Clase == "Economica") ?? 0,
                    Precio = vueloActualizado?.PrecioBase ?? 0m
                },
                new ClaseDisponibilidadDto
                {
                    Clase = "Ejecutiva",
                    AsientosDisponibles = vueloActualizado?.Aeronave?.Asientos?.Count(a => a.Clase == "Ejecutiva") ?? 0,
                    Precio = (vueloActualizado?.PrecioBase ?? 0m) + 100m
                },
                new ClaseDisponibilidadDto
                {
                    Clase = "Primera",
                    AsientosDisponibles = vueloActualizado?.Aeronave?.Asientos?.Count(a => a.Clase == "Primera") ?? 0,
                    Precio = (vueloActualizado?.PrecioBase ?? 0m) + 200m
                }
            };

            if (vueloActualizado?.Aeronave != null)
            {
                vueloDto.Aeronave = new AeronaveInfoDto
                {
                    Matricula = vueloActualizado.Aeronave.Matricula,
                    Modelo = vueloActualizado.Aeronave.Modelo,
                    Capacidad = vueloActualizado.Aeronave.Capacidad,
                    Estado = vueloActualizado.Aeronave.Estado,
                    TiempoPreparacionMinutos = vueloActualizado.Aeronave.TiempoPreparacionMinutos,
                    TotalAsientos = vueloActualizado.Aeronave.Asientos?.Count ?? 0
                };
            }

            return OperationResult<VueloDetalleDto>.SuccessResult(
                vueloDto,
                $"Vuelo actualizado exitosamente.{mensajeNotificacion}"
            );
        }

        /// <summary>
        /// Determina el tipo de cambio basado en las diferencias
        /// </summary>
        private static TipoCambioVuelo DeterminarTipoCambio(
            DateTime fechaAnterior, DateTime fechaNueva,
            TimeSpan horaSalidaAnterior, TimeSpan horaSalidaNueva,
            string? estadoAnterior, string? estadoNuevo)
        {
            // Si el estado cambió a Cancelado
            if (estadoNuevo == "Cancelado")
                return TipoCambioVuelo.Cancelacion;

            // Si cambió la fecha
            if (fechaAnterior.Date != fechaNueva.Date)
                return TipoCambioVuelo.Reprogramacion;

            // Si cambió la hora de salida
            if (horaSalidaAnterior != horaSalidaNueva)
            {
                // Si la nueva hora es después, es un retraso
                if (horaSalidaNueva > horaSalidaAnterior)
                    return TipoCambioVuelo.Retraso;
                else
                    return TipoCambioVuelo.Adelanto;
            }

            return TipoCambioVuelo.CambioEstado;
        }

        public async Task<bool> EliminarVueloAsync(int id)
        {
            var vuelo = await _vueloRepository.GetByIdAsync(id);
            if (vuelo == null)
                return false;

            // ✅ VALIDACIÓN: No se puede eliminar si tiene reservas asociadas
            var reservasAsociadas = await _vueloRepository.Context.Reservas
         .Where(r => r.IdVuelo == id && r.Estado != "Cancelada")
           .CountAsync();

            if (reservasAsociadas > 0)
            {
                throw new InvalidOperationException(
                            $"No se puede eliminar el vuelo '{vuelo.NumeroVuelo}'. " +
                        $"Tiene {reservasAsociadas} reserva(s) activa(s). " +
                        $"Debe cancelar todas las reservas antes de eliminar el vuelo.");
            }

            _vueloRepository.Delete(vuelo);
            await _vueloRepository.SaveAsync();

            return true;
        }

        public async Task<List<VueloDetalleDto>> ObtenerTodosLosVuelosAsync()
        {
            var vuelos = await _vueloRepository.GetAllAsync();
            var vuelosConDetalles = new List<VueloDetalleDto>();

            foreach (var vuelo in vuelos)
            {
                var vueloDetallado = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);

                if (vueloDetallado != null)
                {
                    var vueloDto = _mapper.Map<VueloDetalleDto>(vueloDetallado);

                    // ✅ Agregar información de las 3 clases disponibles con sus precios
                    vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto>
        {
 new ClaseDisponibilidadDto
        {
     Clase = "Economica",
       AsientosDisponibles = vueloDetallado.Aeronave?.Asientos?.Count(a => a.Clase == "Economica") ?? 0,
  Precio = vueloDetallado.PrecioBase
      },
       new ClaseDisponibilidadDto
        {
          Clase = "Ejecutiva",
      AsientosDisponibles = vueloDetallado.Aeronave?.Asientos?.Count(a => a.Clase == "Ejecutiva") ?? 0,
 Precio = vueloDetallado.PrecioBase + 100m
     },
         new ClaseDisponibilidadDto
         {
          Clase = "Primera",
     AsientosDisponibles = vueloDetallado.Aeronave?.Asientos?.Count(a => a.Clase == "Primera") ?? 0,
       Precio = vueloDetallado.PrecioBase + 200m
     }
         };

                    if (vueloDetallado.Aeronave != null)
                    {
                        vueloDto.Aeronave = new AeronaveInfoDto
                        {
                            Matricula = vueloDetallado.Aeronave.Matricula,
                            Modelo = vueloDetallado.Aeronave.Modelo,
                            Capacidad = vueloDetallado.Aeronave.Capacidad,
                            Estado = vueloDetallado.Aeronave.Estado,
                            TiempoPreparacionMinutos = vueloDetallado.Aeronave.TiempoPreparacionMinutos,
                            TotalAsientos = vueloDetallado.Aeronave.Asientos?.Count ?? 0
                        };
                    }

                    vuelosConDetalles.Add(vueloDto);
                }
            }

            return vuelosConDetalles
                .OrderBy(v => v.Fecha)
                      .ThenBy(v => v.HoraSalida)
                    .ToList();
        }
    }
}