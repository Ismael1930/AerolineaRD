using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore; // ? YA EXISTE

namespace AerolineaRD.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IPersonalRepository _personalRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly IAeronaveRepository _aeronaveRepository;
 private readonly IMapper _mapper;

        public EquipoService(
 IPersonalRepository personalRepository,
       IEquipoRepository equipoRepository,
      IAeronaveRepository aeronaveRepository,
            IMapper mapper)
     {
   _personalRepository = personalRepository;
 _equipoRepository = equipoRepository;
  _aeronaveRepository = aeronaveRepository;
     _mapper = mapper;
   }

    // ========== PERSONAL ==========

        public async Task<List<PersonalDto>> ObtenerTodoPersonalAsync()
        {
       var personal = await _personalRepository.GetAllAsync();
      return _mapper.Map<List<PersonalDto>>(personal.Where(p => p.Activo).OrderBy(p => p.Apellido));
 }

     public async Task<List<PersonalDto>> ObtenerPersonalPorRolAsync(string rol)
    {
        var personal = await _personalRepository.ObtenerPorRolAsync(rol);
  return _mapper.Map<List<PersonalDto>>(personal);
  }

    public async Task<List<PersonalDto>> ObtenerPersonalDisponibleAsync()
  {
     var personal = await _personalRepository.ObtenerDisponiblesAsync();
     return _mapper.Map<List<PersonalDto>>(personal);
  }

        public async Task<PersonalDto?> ObtenerPersonalPorIdAsync(int id)
 {
            var personal = await _personalRepository.GetByIdAsync(id);
    return personal == null ? null : _mapper.Map<PersonalDto>(personal);
 }

        public async Task<OperationResult<PersonalDto>> CrearPersonalAsync(CrearPersonalDto dto)
   {
     var errores = new List<ValidationError>();

   // Validar rol
   if (!new[] { "Piloto", "Copiloto", "Sobrecargo Jefe", "Sobrecargo" }.Contains(dto.Rol))
    {
 errores.Add(ValidationError.Create(
 "Rol",
   "DATOS_INVALIDOS",
    $"Rol '{dto.Rol}' no válido. Valores permitidos: Piloto, Copiloto, Sobrecargo Jefe, Sobrecargo"
 ));
            }

            // Validar licencia para pilotos y copilotos
     if ((dto.Rol == "Piloto" || dto.Rol == "Copiloto") && string.IsNullOrEmpty(dto.Licencia))
     {
         errores.Add(ValidationError.Create(
   "Licencia",
    "DATOS_INVALIDOS",
 $"La licencia es obligatoria para {dto.Rol}"
     ));
    }

    // Validar certificaciones para pilotos y copilotos
  if ((dto.Rol == "Piloto" || dto.Rol == "Copiloto") && string.IsNullOrEmpty(dto.CertificacionesAeronave))
 {
     errores.Add(ValidationError.Create(
      "CertificacionesAeronave",
     "DATOS_INVALIDOS",
           $"Las certificaciones son obligatorias para {dto.Rol}"
 ));
 }

      if (errores.Any())
     return OperationResult<PersonalDto>.ValidationFailure(errores);

            var personal = _mapper.Map<Personal>(dto);
 personal.Estado = "Disponible";
   personal.FechaContratacion = DateTime.Today;

      await _personalRepository.AddAsync(personal);
       await _personalRepository.SaveAsync();

     var personalDto = _mapper.Map<PersonalDto>(personal);
    return OperationResult<PersonalDto>.SuccessResult(personalDto, "Personal creado exitosamente");
    }

     public async Task<OperationResult<PersonalDto>> ActualizarPersonalAsync(ActualizarPersonalDto dto)
        {
            var errores = new List<ValidationError>();

   var personal = await _personalRepository.GetByIdAsync(dto.Id);
  if (personal == null)
 {
     errores.Add(ValidationError.Create("Id", "ENTIDAD_NO_ENCONTRADA", $"Personal con ID {dto.Id} no encontrado"));
      return OperationResult<PersonalDto>.ValidationFailure(errores);
    }

      // Actualizar solo los campos proporcionados
    if (!string.IsNullOrEmpty(dto.Nombre)) personal.Nombre = dto.Nombre;
       if (!string.IsNullOrEmpty(dto.Apellido)) personal.Apellido = dto.Apellido;
   if (!string.IsNullOrEmpty(dto.Rol)) personal.Rol = dto.Rol;
     if (dto.Licencia != null) personal.Licencia = dto.Licencia;
            if (dto.CertificacionesAeronave != null) personal.CertificacionesAeronave = dto.CertificacionesAeronave;
 if (dto.TiempoDescansoMinutos.HasValue) personal.TiempoDescansoMinutos = dto.TiempoDescansoMinutos.Value;
   if (!string.IsNullOrEmpty(dto.Estado)) personal.Estado = dto.Estado;

    _personalRepository.Update(personal);
            await _personalRepository.SaveAsync();

    var personalDto = _mapper.Map<PersonalDto>(personal);
      return OperationResult<PersonalDto>.SuccessResult(personalDto, "Personal actualizado exitosamente");
 }

        public async Task<bool> EliminarPersonalAsync(int id)
     {
   var personal = await _personalRepository.GetByIdAsync(id);
    if (personal == null)
    return false;

     // ? VALIDACIÓN: No se puede eliminar si pertenece a un equipo activo
        var perteneceAEquipo = await _personalRepository.Context.EquipoPersonal
     .AnyAsync(ep => ep.IdPersonal == id && ep.Activo);

          if (perteneceAEquipo)
       {
 throw new InvalidOperationException(
         $"No se puede eliminar el personal '{personal.Nombre} {personal.Apellido}'. " +
          $"Actualmente pertenece a un equipo activo. " +
       $"Debe remover al personal del equipo antes de eliminarlo.");
    }

            // Soft delete
       personal.Activo = false;
  _personalRepository.Update(personal);
            await _personalRepository.SaveAsync();

        return true;
        }

        // ========== EQUIPOS ==========

        public async Task<List<EquipoDto>> ObtenerTodosEquiposAsync()
  {
   var equipos = await _equipoRepository.GetAllAsync();
       var equiposActivos = equipos.Where(e => e.Activo).ToList();

  var equiposDto = new List<EquipoDto>();
            foreach (var equipo in equiposActivos)
  {
  var equipoCompleto = await _equipoRepository.ObtenerConMiembrosYAsignacionAsync(equipo.Id);
  if (equipoCompleto != null)
    {
      var dto = MapearEquipoADto(equipoCompleto);
equiposDto.Add(dto);
                }
         }

            return equiposDto.OrderBy(e => e.Nombre).ToList();
        }

     public async Task<List<EquipoDto>> ObtenerEquiposDisponiblesAsync()
        {
       var equipos = await _equipoRepository.ObtenerDisponiblesAsync();
            return equipos.Select(e => MapearEquipoADto(e)).ToList();
        }

        public async Task<EquipoDetalleDto?> ObtenerEquipoPorIdAsync(int id)
        {
       var equipo = await _equipoRepository.ObtenerConMiembrosYAsignacionAsync(id);
   if (equipo == null)
       return null;

  return MapearEquipoADetalleDto(equipo);
     }

  public async Task<OperationResult<EquipoDetalleDto>> CrearEquipoAsync(CrearEquipoDto dto)
        {
            var errores = new List<ValidationError>();

   // Validar código único
        if (await _equipoRepository.CodigoExisteAsync(dto.Codigo))
         {
    errores.Add(ValidationError.Create(
       "Codigo",
       "DATOS_INVALIDOS",
      $"Ya existe un equipo con el código '{dto.Codigo}'"
));
    }

// Validar composición del equipo
    var validacion = await ValidarComposicionEquipoAsync(dto.IdsPersonal);
            if (!validacion.EsValido)
            {
 errores.AddRange(validacion.Errores.Select(e => ValidationError.Create(
  "IdsPersonal",
     "COMPOSICION_INVALIDA",
             e
    )));
            }

  if (errores.Any())
      return OperationResult<EquipoDetalleDto>.ValidationFailure(errores);

  // Crear equipo
       var equipo = new Equipo
            {
       Nombre = dto.Nombre,
        Codigo = dto.Codigo,
           Estado = validacion.EsValido ? "Disponible" : "Incompleto",
       FechaCreacion = DateTime.Now,
     Activo = true
            };

         await _equipoRepository.AddAsync(equipo);
     await _equipoRepository.SaveAsync();

 // Asignar miembros
     if (dto.IdsPersonal.Any())
          {
                await _equipoRepository.AsignarMiembrosAsync(equipo.Id, dto.IdsPersonal);
   await _equipoRepository.SaveAsync();
            }

         var equipoCreado = await _equipoRepository.ObtenerConMiembrosYAsignacionAsync(equipo.Id);
            var equipoDto = MapearEquipoADetalleDto(equipoCreado!);

            return OperationResult<EquipoDetalleDto>.SuccessResult(
      equipoDto,
          "Equipo creado exitosamente"
 );
        }

        public async Task<OperationResult<EquipoDetalleDto>> ActualizarEquipoAsync(ActualizarEquipoDto dto)
        {
            var errores = new List<ValidationError>();

      var equipo = await _equipoRepository.GetByIdAsync(dto.Id);
            if (equipo == null)
       {
  errores.Add(ValidationError.Create("Id", "ENTIDAD_NO_ENCONTRADA", $"Equipo con ID {dto.Id} no encontrado"));
           return OperationResult<EquipoDetalleDto>.ValidationFailure(errores);
  }

      // Validar código único si se está cambiando
       if (!string.IsNullOrEmpty(dto.Codigo) && dto.Codigo != equipo.Codigo)
            {
 if (await _equipoRepository.CodigoExisteAsync(dto.Codigo, dto.Id))
        {
 errores.Add(ValidationError.Create(
   "Codigo",
"DATOS_INVALIDOS",
      $"Ya existe un equipo con el código '{dto.Codigo}'"
   ));
    }
   }

       // Validar composición si se están actualizando los miembros
   if (dto.IdsPersonal != null && dto.IdsPersonal.Any())
{
var validacion = await ValidarComposicionEquipoAsync(dto.IdsPersonal);
        if (!validacion.EsValido)
   {
     errores.AddRange(validacion.Errores.Select(e => ValidationError.Create(
            "IdsPersonal",
          "COMPOSICION_INVALIDA",
         e
              )));
                }

         if (errores.Any())
      return OperationResult<EquipoDetalleDto>.ValidationFailure(errores);

 // Actualizar miembros
      await _equipoRepository.DesasignarTodosMiembrosAsync(dto.Id);
            await _equipoRepository.AsignarMiembrosAsync(dto.Id, dto.IdsPersonal);

    equipo.Estado = validacion.EsValido ? "Disponible" : "Incompleto";
            }

  // Actualizar campos básicos
    if (!string.IsNullOrEmpty(dto.Nombre)) equipo.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Codigo)) equipo.Codigo = dto.Codigo;

  _equipoRepository.Update(equipo);
            await _equipoRepository.SaveAsync();

     var equipoActualizado = await _equipoRepository.ObtenerConMiembrosYAsignacionAsync(dto.Id);
    var equipoDto = MapearEquipoADetalleDto(equipoActualizado!);

         return OperationResult<EquipoDetalleDto>.SuccessResult(
             equipoDto,
    "Equipo actualizado exitosamente"
            );
 }

        public async Task<bool> EliminarEquipoAsync(int id)
        {
            var equipo = await _equipoRepository.GetByIdAsync(id);
   if (equipo == null)
      return false;

    // Verificar que no tenga asignación activa
      var asignacionActiva = await _equipoRepository.ObtenerAsignacionActivaPorEquipoAsync(id);
            if (asignacionActiva != null)
                return false;

            // Soft delete
  equipo.Activo = false;
 await _equipoRepository.DesasignarTodosMiembrosAsync(id);
 
      _equipoRepository.Update(equipo);
       await _equipoRepository.SaveAsync();

         return true;
  }

   public async Task<ValidacionEquipoDto> ValidarComposicionEquipoAsync(List<int> idsPersonal)
        {
          var resultado = new ValidacionEquipoDto
            {
                EsValido = true,
       Errores = new List<string>(),
   Composicion = new ComposicionEquipoDto()
            };

            if (!idsPersonal.Any())
       {
          resultado.EsValido = false;
    resultado.Errores.Add("El equipo debe tener al menos un miembro");
                return resultado;
     }

     var personal = await _personalRepository.ObtenerPorIdsAsync(idsPersonal);

            // Contar por rol
        var pilotos = personal.Count(p => p.Rol == "Piloto");
 var copilotos = personal.Count(p => p.Rol == "Copiloto");
    var sobrecargosJefe = personal.Count(p => p.Rol == "Sobrecargo Jefe");
            var sobrecargos = personal.Count(p => p.Rol == "Sobrecargo");

resultado.Composicion.TotalPilotos = pilotos;
     resultado.Composicion.TotalCopilotos = copilotos;
        resultado.Composicion.TotalSobrecargosJefe = sobrecargosJefe;
            resultado.Composicion.TotalSobrecargos = sobrecargos;
            resultado.Composicion.TotalMiembros = personal.Count;

      // Validar composición mínima
     if (pilotos != 1)
            {
  resultado.EsValido = false;
       resultado.Errores.Add($"El equipo debe tener exactamente 1 Piloto (tiene {pilotos})");
     }

   if (copilotos != 1)
    {
          resultado.EsValido = false;
      resultado.Errores.Add($"El equipo debe tener exactamente 1 Copiloto (tiene {copilotos})");
          }

     if (sobrecargosJefe != 1)
  {
          resultado.EsValido = false;
      resultado.Errores.Add($"El equipo debe tener exactamente 1 Sobrecargo Jefe (tiene {sobrecargosJefe})");
            }

   if (sobrecargos < 3 || sobrecargos > 6)
            {
  resultado.EsValido = false;
      resultado.Errores.Add($"El equipo debe tener entre 3 y 6 Sobrecargos (tiene {sobrecargos})");
  }

            return resultado;
     }

        // ========== ASIGNACIONES ==========

        public async Task<List<AsignacionAeronaveDto>> ObtenerTodasAsignacionesAsync()
     {
    var asignaciones = await _equipoRepository.ObtenerTodasAsignacionesAsync();
    return asignaciones.Select(a => MapearAsignacionADto(a)).ToList();
        }

    public async Task<AsignacionAeronaveDto?> ObtenerAsignacionActivaPorAeronaveAsync(string matricula)
        {
      var asignacion = await _equipoRepository.ObtenerAsignacionActivaPorAeronaveAsync(matricula);
            return asignacion == null ? null : MapearAsignacionADto(asignacion);
        }

        public async Task<AsignacionAeronaveDto?> ObtenerAsignacionActivaPorEquipoAsync(int idEquipo)
        {
            var asignacion = await _equipoRepository.ObtenerAsignacionActivaPorEquipoAsync(idEquipo);
       return asignacion == null ? null : MapearAsignacionADto(asignacion);
      }

        public async Task<OperationResult<AsignacionAeronaveDto>> AsignarEquipoAeronaveAsync(AsignarEquipoAeronaveDto dto)
      {
   var errores = new List<ValidationError>();

            // Validar que el equipo existe
  var equipo = await _equipoRepository.ObtenerConMiembrosAsync(dto.IdEquipo);
            if (equipo == null)
            {
       errores.Add(ValidationError.Create("IdEquipo", "ENTIDAD_NO_ENCONTRADA", $"Equipo con ID {dto.IdEquipo} no encontrado"));
                return OperationResult<AsignacionAeronaveDto>.ValidationFailure(errores);
         }

    // Validar que la aeronave existe
            var aeronave = await _aeronaveRepository.GetByIdAsync(dto.Matricula);
            if (aeronave == null)
            {
   errores.Add(ValidationError.Create("Matricula", "ENTIDAD_NO_ENCONTRADA", $"Aeronave con matrícula '{dto.Matricula}' no encontrada"));
           return OperationResult<AsignacionAeronaveDto>.ValidationFailure(errores);
          }

            // Validar que la aeronave esté operativa
     if (aeronave.Estado != "Operativa")
      {
     errores.Add(ValidationError.Create(
   "Matricula",
    "AERONAVE_NO_DISPONIBLE",
          $"La aeronave '{dto.Matricula}' no está operativa (Estado: {aeronave.Estado})"
        ));
  }

  // Validar que el equipo esté disponible
            if (equipo.Estado != "Disponible")
            {
          errores.Add(ValidationError.Create(
  "IdEquipo",
       "EQUIPO_NO_DISPONIBLE",
      $"El equipo '{equipo.Nombre}' no está disponible (Estado: {equipo.Estado})"
                ));
       }

 // Validar composición del equipo
            var idsPersonal = equipo.EquiposPersonal.Where(ep => ep.Activo).Select(ep => ep.IdPersonal).ToList();
         var validacion = await ValidarComposicionEquipoAsync(idsPersonal);
  if (!validacion.EsValido)
 {
     errores.Add(ValidationError.Create(
        "IdEquipo",
   "EQUIPO_INCOMPLETO",
    $"El equipo no tiene la composición requerida: {string.Join(", ", validacion.Errores)}"
    ));
  }

         // ?? VALIDACIONES DE CERTIFICACIONES ELIMINADAS
         // La lógica de certificaciones se manejará en el futuro

         // Validar que la aeronave no tenga otra asignación activa
       var asignacionExistente = await _equipoRepository.ObtenerAsignacionActivaPorAeronaveAsync(dto.Matricula);
            if (asignacionExistente != null)
            {
                errores.Add(ValidationError.Create(
        "Matricula",
  "AERONAVE_YA_ASIGNADA",
       $"La aeronave '{dto.Matricula}' ya tiene asignado el equipo '{asignacionExistente.Equipo.Nombre}'"
                ));
            }

            // Validar que el equipo no tenga otra asignación activa
            var equipoYaAsignado = await _equipoRepository.ObtenerAsignacionActivaPorEquipoAsync(dto.IdEquipo);
       if (equipoYaAsignado != null)
    {
         errores.Add(ValidationError.Create(
         "IdEquipo",
   "EQUIPO_YA_ASIGNADO",
        $"El equipo '{equipo.Nombre}' ya está asignado a la aeronave '{equipoYaAsignado.Matricula}'"
));
            }

        if (errores.Any())
        return OperationResult<AsignacionAeronaveDto>.ValidationFailure(errores);

        // Crear asignación
    var asignacion = new AsignacionEquipoAeronave
            {
  IdEquipo = dto.IdEquipo,
Matricula = dto.Matricula,
     FechaAsignacion = DateTime.Now,
  Activa = true,
           Observaciones = dto.Observaciones
          };

await _equipoRepository.Context.AsignacionesEquipoAeronave.AddAsync(asignacion);
      
            // Actualizar estado del equipo
     equipo.Estado = "Disponible"; // Se marca como disponible pero asignado
       _equipoRepository.Update(equipo);

       await _equipoRepository.SaveAsync();

       var asignacionCreada = await _equipoRepository.ObtenerAsignacionActivaPorAeronaveAsync(dto.Matricula);
         var asignacionDto = MapearAsignacionADto(asignacionCreada!);

            return OperationResult<AsignacionAeronaveDto>.SuccessResult(
       asignacionDto,
      $"Equipo '{equipo.Nombre}' asignado exitosamente a aeronave '{dto.Matricula}'"
);
     }

        public async Task<OperationResult<AsignacionAeronaveDto>> DesasignarEquipoAeronaveAsync(DesasignarEquipoDto dto)
   {
  var errores = new List<ValidationError>();

            var asignacion = await _equipoRepository.Context.AsignacionesEquipoAeronave
    .Include(a => a.Equipo)
           .Include(a => a.Aeronave)
       .FirstOrDefaultAsync(a => a.Id == dto.IdAsignacion);

     if (asignacion == null)
          {
         errores.Add(ValidationError.Create("IdAsignacion", "ENTIDAD_NO_ENCONTRADA", $"Asignación con ID {dto.IdAsignacion} no encontrada"));
          return OperationResult<AsignacionAeronaveDto>.ValidationFailure(errores);
      }

    if (!asignacion.Activa)
            {
       errores.Add(ValidationError.Create("IdAsignacion", "ASIGNACION_INACTIVA", "La asignación ya está inactiva"));
                return OperationResult<AsignacionAeronaveDto>.ValidationFailure(errores);
     }

            // Desactivar asignación
         asignacion.Activa = false;
         asignacion.FechaDesasignacion = DateTime.Now;
 if (!string.IsNullOrEmpty(dto.Observaciones))
          {
                asignacion.Observaciones += $" | Desasignación: {dto.Observaciones}";
         }

   await _equipoRepository.SaveAsync();

            var asignacionDto = MapearAsignacionADto(asignacion);

   return OperationResult<AsignacionAeronaveDto>.SuccessResult(
      asignacionDto,
          $"Equipo desasignado exitosamente de aeronave '{asignacion.Matricula}'"
    );
        }

        // ========== GESTIÓN DE ESTADOS ==========

        public async Task<OperationResult<string>> MarcarEquipoEnServicioAsync(int idEquipo)
        {
            var equipo = await _equipoRepository.GetByIdAsync(idEquipo);
     if (equipo == null)
   {
   return OperationResult<string>.ValidationFailure(new List<ValidationError>
     {
 ValidationError.Create("IdEquipo", "ENTIDAD_NO_ENCONTRADA", $"Equipo con ID {idEquipo} no encontrado")
     });
            }

            equipo.Estado = "En Servicio";
       _equipoRepository.Update(equipo);
            await _equipoRepository.SaveAsync();

  // Actualizar estado del personal
  var miembros = await _equipoRepository.Context.EquipoPersonal
        .Where(ep => ep.IdEquipo == idEquipo && ep.Activo)
                .Include(ep => ep.Personal)
    .ToListAsync();

 foreach (var miembro in miembros)
   {
              miembro.Personal.Estado = "En Servicio";
            }

            await _equipoRepository.SaveAsync();

  return OperationResult<string>.SuccessResult(
          "En Servicio",
                $"Equipo '{equipo.Nombre}' marcado como En Servicio"
     );
        }

        public async Task<OperationResult<string>> MarcarEquipoEnDescansoAsync(int idEquipo, DateTime finVuelo)
        {
            var equipo = await _equipoRepository.GetByIdAsync(idEquipo);
      if (equipo == null)
            {
     return OperationResult<string>.ValidationFailure(new List<ValidationError>
      {
     ValidationError.Create("IdEquipo", "ENTIDAD_NO_ENCONTRADA", $"Equipo con ID {idEquipo} no encontrado")
    });
            }

            equipo.Estado = "Descanso";
       equipo.UltimoVueloFin = finVuelo;
        equipo.DisponibleDesde = finVuelo.AddMinutes(480); // 8 horas de descanso

            _equipoRepository.Update(equipo);
  await _equipoRepository.SaveAsync();

       // Actualizar estado del personal
   var miembros = await _equipoRepository.Context.EquipoPersonal
 .Where(ep => ep.IdEquipo == idEquipo && ep.Activo)
        .Include(ep => ep.Personal)
         .ToListAsync();

            foreach (var miembro in miembros)
  {
     miembro.Personal.Estado = "Descanso";
                miembro.Personal.UltimoVueloFin = finVuelo;
     }

            await _equipoRepository.SaveAsync();

      return OperationResult<string>.SuccessResult(
  "Descanso",
                $"Equipo '{equipo.Nombre}' en descanso hasta {equipo.DisponibleDesde:dd/MM/yyyy HH:mm}"
   );
        }

     public async Task ActualizarEstadosEquiposAsync()
        {
      var ahora = DateTime.Now;
            var equiposEnDescanso = await _equipoRepository.Context.Equipos
    .Where(e => e.Estado == "Descanso" && e.DisponibleDesde != null && e.DisponibleDesde <= ahora)
        .ToListAsync();

  foreach (var equipo in equiposEnDescanso)
            {
     equipo.Estado = "Disponible";
           
           // Actualizar personal del equipo
     var miembros = await _equipoRepository.Context.EquipoPersonal
.Where(ep => ep.IdEquipo == equipo.Id && ep.Activo)
         .Include(ep => ep.Personal)
   .ToListAsync();

      foreach (var miembro in miembros)
           {
     if (miembro.Personal.UltimoVueloFin != null)
       {
     var minutosDescanso = (ahora - miembro.Personal.UltimoVueloFin.Value).TotalMinutes;
 if (minutosDescanso >= miembro.Personal.TiempoDescansoMinutos)
 {
                miembro.Personal.Estado = "Disponible";
      }
          }
     }
   }

 await _equipoRepository.SaveAsync();
  }

      // ========== MÉTODOS AUXILIARES DE MAPEO ==========

     private EquipoDto MapearEquipoADto(Equipo equipo)
      {
      var dto = _mapper.Map<EquipoDto>(equipo);
        
     if (equipo.EquiposPersonal != null && equipo.EquiposPersonal.Any())
          {
                dto.Miembros = equipo.EquiposPersonal
  .Where(ep => ep.Activo)
           .Select(ep => new PersonalDto
           {
      Id = ep.Personal.Id,
          Nombre = ep.Personal.Nombre,
       Apellido = ep.Personal.Apellido,
            Rol = ep.Personal.Rol,
      Licencia = ep.Personal.Licencia,
            CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
   TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
     Estado = ep.Personal.Estado,
      UltimoVueloFin = ep.Personal.UltimoVueloFin,
 FechaContratacion = ep.Personal.FechaContratacion,
    Activo = ep.Personal.Activo
           })
        .ToList();
      }

            // ?? NO mapear AsignacionActual para evitar referencias circulares
// Se puede agregar solo IDs básicos si es necesario
    dto.AsignacionActual = null;

      return dto;
 }

        private EquipoDetalleDto MapearEquipoADetalleDto(Equipo equipo)
        {
            var dto = _mapper.Map<EquipoDetalleDto>(equipo);
     
            var miembrosActivos = equipo.EquiposPersonal?.Where(ep => ep.Activo).ToList() ?? new List<EquipoPersonal>();
  
            dto.Miembros = miembrosActivos
 .Select(ep => new PersonalDto
  {
 Id = ep.Personal.Id,
             Nombre = ep.Personal.Nombre,
                Apellido = ep.Personal.Apellido,
           Rol = ep.Personal.Rol,
     Licencia = ep.Personal.Licencia,
    CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
   TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
 Estado = ep.Personal.Estado,
         UltimoVueloFin = ep.Personal.UltimoVueloFin,
  FechaContratacion = ep.Personal.FechaContratacion,
       Activo = ep.Personal.Activo
                })
 .ToList();

            dto.Piloto = miembrosActivos
    .Where(ep => ep.Personal.Rol == "Piloto")
  .Select(ep => new PersonalDto
                {
     Id = ep.Personal.Id,
             Nombre = ep.Personal.Nombre,
           Apellido = ep.Personal.Apellido,
         Rol = ep.Personal.Rol,
  Licencia = ep.Personal.Licencia,
      CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
        TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
     Estado = ep.Personal.Estado,
           UltimoVueloFin = ep.Personal.UltimoVueloFin,
         FechaContratacion = ep.Personal.FechaContratacion,
   Activo = ep.Personal.Activo
    })
     .FirstOrDefault();

    dto.Copiloto = miembrosActivos
         .Where(ep => ep.Personal.Rol == "Copiloto")
      .Select(ep => new PersonalDto
     {
    Id = ep.Personal.Id,
        Nombre = ep.Personal.Nombre,
       Apellido = ep.Personal.Apellido,
    Rol = ep.Personal.Rol,
     Licencia = ep.Personal.Licencia,
     CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
   Estado = ep.Personal.Estado,
       UltimoVueloFin = ep.Personal.UltimoVueloFin,
     FechaContratacion = ep.Personal.FechaContratacion,
           Activo = ep.Personal.Activo
    })
             .FirstOrDefault();

       dto.SobrecargoJefe = miembrosActivos
         .Where(ep => ep.Personal.Rol == "Sobrecargo Jefe")
                .Select(ep => new PersonalDto
        {
  Id = ep.Personal.Id,
          Nombre = ep.Personal.Nombre,
  Apellido = ep.Personal.Apellido,
      Rol = ep.Personal.Rol,
    Licencia = ep.Personal.Licencia,
                CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
            TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
            Estado = ep.Personal.Estado,
      UltimoVueloFin = ep.Personal.UltimoVueloFin,
FechaContratacion = ep.Personal.FechaContratacion,
   Activo = ep.Personal.Activo
          })
           .FirstOrDefault();

 dto.Sobrecargos = miembrosActivos
             .Where(ep => ep.Personal.Rol == "Sobrecargo")
         .Select(ep => new PersonalDto
        {
         Id = ep.Personal.Id,
  Nombre = ep.Personal.Nombre,
             Apellido = ep.Personal.Apellido,
   Rol = ep.Personal.Rol,
          Licencia = ep.Personal.Licencia,
           CertificacionesAeronave = ep.Personal.CertificacionesAeronave,
         TiempoDescansoMinutos = ep.Personal.TiempoDescansoMinutos,
        Estado = ep.Personal.Estado,
       UltimoVueloFin = ep.Personal.UltimoVueloFin,
  FechaContratacion = ep.Personal.FechaContratacion,
      Activo = ep.Personal.Activo
   })
     .ToList();

            var validacion = ValidarComposicionEquipoAsync(
  miembrosActivos.Select(ep => ep.IdPersonal).ToList()
        ).GetAwaiter().GetResult();

            dto.EsEquipoCompleto = validacion.EsValido;
            dto.MensajeValidacion = validacion.EsValido 
     ? "Equipo completo y listo para asignar" 
         : string.Join(", ", validacion.Errores);

        // Mapear asignación activa de forma segura
        if (equipo.AsignacionesAeronave != null)
{
   var asignacionActiva = equipo.AsignacionesAeronave.FirstOrDefault(a => a.Activa);
        if (asignacionActiva != null)
            {
         dto.AsignacionActual = new AsignacionAeronaveDto
        {
   Id = asignacionActiva.Id,
         IdEquipo = asignacionActiva.IdEquipo,
        Matricula = asignacionActiva.Matricula,
            FechaAsignacion = asignacionActiva.FechaAsignacion,
    FechaDesasignacion = asignacionActiva.FechaDesasignacion,
   Activa = asignacionActiva.Activa,
      Observaciones = asignacionActiva.Observaciones,
           Aeronave = asignacionActiva.Aeronave != null ? new AeronaveInfoDto
            {
                  Matricula = asignacionActiva.Aeronave.Matricula,
          Modelo = asignacionActiva.Aeronave.Modelo,
  Capacidad = asignacionActiva.Aeronave.Capacidad,
             Estado = asignacionActiva.Aeronave.Estado,
             TiempoPreparacionMinutos = asignacionActiva.Aeronave.TiempoPreparacionMinutos
                } : null,
   Equipo = null // ?? NO incluir equipo para evitar loop
        };
        }
 }

    return dto;
      }

        private AsignacionAeronaveDto MapearAsignacionADto(AsignacionEquipoAeronave asignacion)
     {
            var dto = new AsignacionAeronaveDto
     {
                Id = asignacion.Id,
   IdEquipo = asignacion.IdEquipo,
              Matricula = asignacion.Matricula,
          FechaAsignacion = asignacion.FechaAsignacion,
    FechaDesasignacion = asignacion.FechaDesasignacion,
      Activa = asignacion.Activa,
          Observaciones = asignacion.Observaciones
     };

            // Mapear equipo de forma simplificada (sin asignaciones anidadas)
       if (asignacion.Equipo != null)
{
          dto.Equipo = new EquipoDto
        {
      Id = asignacion.Equipo.Id,
   Nombre = asignacion.Equipo.Nombre,
        Codigo = asignacion.Equipo.Codigo,
 Estado = asignacion.Equipo.Estado,
  FechaCreacion = asignacion.Equipo.FechaCreacion,
       UltimoVueloFin = asignacion.Equipo.UltimoVueloFin,
           DisponibleDesde = asignacion.Equipo.DisponibleDesde,
     Activo = asignacion.Equipo.Activo,
  Miembros = asignacion.Equipo.EquiposPersonal?
               .Where(ep => ep.Activo)
    .Select(ep => new PersonalDto
    {
          Id = ep.Personal.Id,
      Nombre = ep.Personal.Nombre,
   Apellido = ep.Personal.Apellido,
          Rol = ep.Personal.Rol,
            Licencia = ep.Personal.Licencia,
     Estado = ep.Personal.Estado
           })
   .ToList() ?? new List<PersonalDto>(),
      AsignacionActual = null // ?? NO incluir para evitar loop
              };
   }

            if (asignacion.Aeronave != null)
            {
   dto.Aeronave = new AeronaveInfoDto
    {
    Matricula = asignacion.Aeronave.Matricula,
               Modelo = asignacion.Aeronave.Modelo,
Capacidad = asignacion.Aeronave.Capacidad,
 Estado = asignacion.Aeronave.Estado,
         TiempoPreparacionMinutos = asignacion.Aeronave.TiempoPreparacionMinutos
  };
  }

 return dto;
        }
    }
}
