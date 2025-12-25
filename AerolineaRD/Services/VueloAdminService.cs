using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class VueloAdminService : IVueloAdminService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IEstadoVueloRepository _estadoVueloRepository;
        private readonly ITripulacionRepository _tripulacionRepository;
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly IMapper _mapper;

        public VueloAdminService(
            IVueloRepository vueloRepository,
            IEstadoVueloRepository estadoVueloRepository,
            ITripulacionRepository tripulacionRepository,
            IAeronaveRepository aeronaveRepository,
            IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _estadoVueloRepository = estadoVueloRepository;
            _tripulacionRepository = tripulacionRepository;
            _aeronaveRepository = aeronaveRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<VueloDetalleDto>> CrearVueloAsync(CrearVueloDto dto)
        {
            var errores = new List<ValidationError>();

            // ? 1. VALIDACIÓN: Verificar disponibilidad de aeronave
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

            var vuelo = _mapper.Map<Vuelo>(dto);
            vuelo.Estado = "Programado";

            // ? Asignar clase del vuelo
            if (!string.IsNullOrEmpty(dto.Clase))
            {
                vuelo.Clase = dto.Clase;
            }

            // ? 4. VALIDACIÓN: Asignar y validar tripulación
            if (dto.IdsTripulacion != null && dto.IdsTripulacion.Any())
            {
                Aeronave? aeronave = null;
                if (!string.IsNullOrEmpty(dto.Matricula))
                {
                    aeronave = await _aeronaveRepository.GetByIdAsync(dto.Matricula);
                }

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

                    // Verificar certificación para pilotos y copilotos
                    if ((tripulacion.Rol == "Piloto" || tripulacion.Rol == "Copiloto") && aeronave != null)
                    {
                        bool tieneCertificacion = await _tripulacionRepository.TieneCertificacionParaAeronaveAsync(
                            idTripulacion,
                            aeronave.Modelo ?? "");

                        if (!tieneCertificacion)
                        {
                            errores.Add(ValidationError.Create(
                                campo: "IdsTripulacion",
                                tipo: ValidationErrorType.TripulanteSinCertificacion,
                                mensaje: $"El {tripulacion.Rol} {tripulacion.Nombre} {tripulacion.Apellido} no tiene la certificación requerida " +
                                         $"para operar aeronaves de tipo {aeronave.Modelo}.",
                                detalles: new
                                {
                                    idTripulacion,
                                    nombre = $"{tripulacion.Nombre} {tripulacion.Apellido}",
                                    rol = tripulacion.Rol,
                                    modeloAeronave = aeronave.Modelo,
                                    certificaciones = tripulacion.CertificacionesAeronave
                                }
                            ));
                            continue;
                        }
                    }

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

            return OperationResult<VueloDetalleDto>.SuccessResult(
                vueloDto,
                "Vuelo creado exitosamente"
            );
        }

        public async Task<VueloDetalleDto?> ObtenerVueloDetalleAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);
            
            if (vuelo == null) 
                 return null;

            var vueloDto = _mapper.Map<VueloDetalleDto>(vuelo);
   
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

            // ? Validación: Si se está cambiando la aeronave o el horario, verificar disponibilidad
            bool cambioAeronave = !string.IsNullOrEmpty(dto.Matricula) && dto.Matricula != vuelo.Matricula;
            bool cambioFecha = dto.Fecha.HasValue && dto.Fecha.Value.Date != vuelo.Fecha.Date;
            bool cambioHoraSalida = dto.HoraSalida.HasValue && dto.HoraSalida.Value != vuelo.HoraSalida;
            bool cambioHoraLlegada = dto.HoraLlegada.HasValue && dto.HoraLlegada.Value != vuelo.HoraLlegada;

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
                        dto.Id); // Excluir el vuelo actual de la validación

                    if (!aeronaveDisponible)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "Matricula",
                            tipo: ValidationErrorType.AeronaveNoDisponible,
                            mensaje: $"La aeronave con matrícula '{matriculaValidar}' no está disponible en el horario especificado. " +
                                     $"Ya tiene un vuelo asignado que se solapa con el horario {horaSalidaValidar:hh\\:mm} - {horaLlegadaValidar:hh\\:mm} el {fechaValidar:dd/MM/yyyy}. " +
                                     $"Recuerde que se requiere un tiempo mínimo de preparación entre vuelos.",
                            detalles: new
                            {
                                matricula = matriculaValidar,
                                fecha = fechaValidar,
                                horaSalida = horaSalidaValidar,
                                horaLlegada = horaLlegadaValidar,
                                vueloId = dto.Id
                            }
                        ));
                    }
                }

                // Validar capacidad de origen si cambió
                if (!string.IsNullOrEmpty(dto.OrigenCodigo))
                {
                    bool origenTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                        dto.OrigenCodigo,
                        fechaValidar,
                        horaSalidaValidar,
                        true);

                    if (!origenTieneCapacidad)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "OrigenCodigo",
                            tipo: ValidationErrorType.AeropuertoSinCapacidad,
                            mensaje: $"El aeropuerto de origen '{dto.OrigenCodigo}' ha alcanzado su capacidad máxima de despegues " +
                                     $"en el horario {horaSalidaValidar:hh\\:mm} el {fechaValidar:dd/MM/yyyy}.",
                            detalles: new
                            {
                                codigoAeropuerto = dto.OrigenCodigo,
                                fecha = fechaValidar,
                                horario = horaSalidaValidar
                            }
                        ));
                    }
                }

                // Validar capacidad de destino si cambió
                if (!string.IsNullOrEmpty(dto.DestinoCodigo))
                {
                    bool destinoTieneCapacidad = await _vueloRepository.AeropuertoTieneCapacidadAsync(
                        dto.DestinoCodigo,
                        fechaValidar,
                        horaLlegadaValidar,
                        false);

                    if (!destinoTieneCapacidad)
                    {
                        errores.Add(ValidationError.Create(
                            campo: "DestinoCodigo",
                            tipo: ValidationErrorType.AeropuertoSinCapacidad,
                            mensaje: $"El aeropuerto de destino '{dto.DestinoCodigo}' ha alcanzado su capacidad máxima de aterrizajes " +
                                     $"en el horario {horaLlegadaValidar:hh\\:mm} el {fechaValidar:dd/MM/yyyy}.",
                            detalles: new
                            {
                                codigoAeropuerto = dto.DestinoCodigo,
                                fecha = fechaValidar,
                                horario = horaLlegadaValidar
                            }
                        ));
                    }
                }
            }

            // ? Si hay errores de validación, devolver resultado fallido
            if (errores.Any())
            {
                return OperationResult<VueloDetalleDto>.ValidationFailure(errores);
            }

            // Actualizar campos básicos
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

            _vueloRepository.Update(vuelo);
            await _vueloRepository.SaveAsync();

   var vueloActualizado = await _vueloRepository.ObtenerVueloConDetallesAsync(dto.Id);
       var vueloDto = _mapper.Map<VueloDetalleDto>(vueloActualizado);

    // ? Agregar información de la aeronave si existe
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
       "Vuelo actualizado exitosamente"
   );
      }

        public async Task<bool> EliminarVueloAsync(int id)
        {
            var vuelo = await _vueloRepository.GetByIdAsync(id);
            if (vuelo == null)
                return false;

            _vueloRepository.Delete(vuelo);
            await _vueloRepository.SaveAsync();

            return true;
        }

        public async Task<List<VueloDetalleDto>> ObtenerTodosLosVuelosAsync()
        {
            var vuelos = await _vueloRepository.GetAllAsync();
    
      // Obtener vuelos con detalles incluyendo aeronave
var vuelosConDetalles = new List<VueloDetalleDto>();

     foreach (var vuelo in vuelos)
{
             var vueloDetallado = await _vueloRepository.ObtenerVueloConDetallesAsync(vuelo.Id);
   
  if (vueloDetallado != null)
       {
         var vueloDto = _mapper.Map<VueloDetalleDto>(vueloDetallado);
 
        // ? Agregar información de la aeronave
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