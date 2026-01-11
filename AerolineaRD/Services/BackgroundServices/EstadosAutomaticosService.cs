using AerolineaRD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AerolineaRD.Services.BackgroundServices
{
    public class EstadosAutomaticosService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EstadosAutomaticosService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(1);

    public EstadosAutomaticosService(
    IServiceProvider serviceProvider,
    ILogger<EstadosAutomaticosService> logger)
      {
 _serviceProvider = serviceProvider;
        _logger = logger;
    }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("? Servicio de Estados Automáticos iniciado");
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
            {
          try
          {
      await ActualizarEstadosAsync();
               await Task.Delay(_intervalo, stoppingToken);
      }
     catch (Exception ex)
    {
          _logger.LogError(ex, "? Error al actualizar estados automáticos");
              await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
      }
}

     _logger.LogInformation("?? Servicio de Estados Automáticos detenido");
  }

        private async Task ActualizarEstadosAsync()
        {
   using var scope = _serviceProvider.CreateScope();
 var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
   var ahora = DateTime.Now;

 try
         {
     await ActualizarEstadosVuelosAsync(context, ahora);
     await ActualizarEstadosRecursosAsync(context, ahora);
          await LiberarRecursosAsync(context, ahora);
  await context.SaveChangesAsync();
    }
     catch (Exception ex)
            {
      _logger.LogError(ex, "? Error al actualizar estados");
         }
        }

     private async Task ActualizarEstadosVuelosAsync(AppDbContext context, DateTime ahora)
        {
var vuelosPendientes = await context.Vuelos
   .Where(v => v.Estado != "Completado" && v.Estado != "Cancelado")
      .ToListAsync();

        foreach (var vuelo in vuelosPendientes)
         {
   var horaSalida = vuelo.Fecha.Date.Add(vuelo.HoraSalida);
            var horaLlegada = vuelo.Fecha.Date.Add(vuelo.HoraLlegada);
     var horaCompletado = horaLlegada.AddMinutes(30);
           var estadoAnterior = vuelo.Estado;

    if (ahora >= horaCompletado)
              {
    vuelo.Estado = "Completado";
            }
      else if (ahora >= horaLlegada)
  {
           vuelo.Estado = "Aterrizado";
         }
        else if (ahora >= horaSalida)
   {
     vuelo.Estado = "En Vuelo";
     }

   if (estadoAnterior != vuelo.Estado)
         {
        _logger.LogInformation($"?? Vuelo {vuelo.NumeroVuelo} cambió de '{estadoAnterior}' a '{vuelo.Estado}' " +
                  $"(Salida: {horaSalida:dd/MM/yyyy HH:mm}, Llegada: {horaLlegada:dd/MM/yyyy HH:mm}, Ahora: {ahora:dd/MM/yyyy HH:mm})");
          }
            }
        }

      private async Task ActualizarEstadosRecursosAsync(AppDbContext context, DateTime ahora)
        {
            _logger.LogInformation($"=== INICIANDO ActualizarEstadosRecursosAsync - Hora actual: {ahora:dd/MM/yyyy HH:mm:ss} ===");
      
          // Vuelos EN VUELO
         var vuelosEnVuelo = await context.Vuelos
  .Where(v => v.Estado == "En Vuelo")
  .ToListAsync();

         _logger.LogInformation($"Encontrados {vuelosEnVuelo.Count} vuelos en estado 'En Vuelo'");

       foreach (var vuelo in vuelosEnVuelo)
       {
     var horaSalida = vuelo.Fecha.Date.Add(vuelo.HoraSalida);
       _logger.LogInformation($"Procesando vuelo {vuelo.NumeroVuelo} - Matricula: {vuelo.Matricula}, Hora Salida: {horaSalida:dd/MM/yyyy HH:mm}");
           
           if (ahora >= horaSalida)
           {
      if (!string.IsNullOrEmpty(vuelo.Matricula))
   {
   var aeronave = await context.Aeronaves.FirstOrDefaultAsync(a => a.Matricula == vuelo.Matricula);
     _logger.LogInformation($"Aeronave encontrada: {aeronave?.Matricula ?? "NULL"}, Estado actual: {aeronave?.Estado ?? "NULL"}");
          
         if (aeronave != null && aeronave.Estado != "En Vuelo")
        {
aeronave.Estado = "En Vuelo";
     _logger.LogInformation($"?? Aeronave {aeronave.Matricula} cambió a 'En Vuelo' (Vuelo {vuelo.NumeroVuelo})");
     }
        }

       var asignacion = await context.AsignacionesEquipoAeronave
      .Include(a => a.Equipo)
      .ThenInclude(e => e.EquiposPersonal)
         .ThenInclude(ep => ep.Personal)
.FirstOrDefaultAsync(a => a.Matricula == vuelo.Matricula && a.Activa);

   _logger.LogInformation($"Asignación encontrada: {(asignacion != null ? $"Equipo {asignacion.Equipo.Nombre}" : "NULL")}");

      if (asignacion != null)
        {
      _logger.LogInformation($"Estado actual del equipo: {asignacion.Equipo.Estado}");
            
    if (asignacion.Equipo.Estado != "En Servicio")
   {
          asignacion.Equipo.Estado = "En Servicio";
          _logger.LogInformation($"?? Equipo {asignacion.Equipo.Nombre} cambió a 'En Servicio' (Vuelo {vuelo.NumeroVuelo})");
  }

         _logger.LogInformation($"Personal activo en el equipo: {asignacion.Equipo.EquiposPersonal.Count(ep => ep.Activo)}");

       foreach (var ep in asignacion.Equipo.EquiposPersonal.Where(ep => ep.Activo))
         {
            _logger.LogInformation($"Procesando personal: {ep.Personal.Nombre} {ep.Personal.Apellido}, Estado: {ep.Personal.Estado}");
            
          if (ep.Personal.Estado != "En Servicio")
          {
  ep.Personal.Estado = "En Servicio";
 _logger.LogInformation($"?? {ep.Personal.Nombre} {ep.Personal.Apellido} cambió a 'En Servicio' (Vuelo {vuelo.NumeroVuelo})");
}
  }
      }
      else
  {
                     _logger.LogWarning($"?? NO se encontró asignación de equipo para la aeronave {vuelo.Matricula} del vuelo {vuelo.NumeroVuelo}");
     }
       }
       else
     {
      _logger.LogInformation($"Vuelo {vuelo.NumeroVuelo} aún no ha alcanzado su hora de salida");
         }
}

   _logger.LogInformation("=== Procesando vuelos TERMINADOS ===");
            
         // Vuelos TERMINADOS
         var vuelosTerminados = await context.Vuelos
     .Where(v => v.Estado == "Aterrizado" || v.Estado == "Completado")
    .ToListAsync();

 _logger.LogInformation($"Encontrados {vuelosTerminados.Count} vuelos terminados (Aterrizado o Completado)");

       foreach (var vuelo in vuelosTerminados)
        {
    var horaLlegada = vuelo.Fecha.Date.Add(vuelo.HoraLlegada);
      _logger.LogInformation($"Procesando vuelo terminado {vuelo.NumeroVuelo} - Hora Llegada: {horaLlegada:dd/MM/yyyy HH:mm}");
      
 if (ahora >= horaLlegada)
  {
   if (!string.IsNullOrEmpty(vuelo.Matricula))
        {
        var aeronave = await context.Aeronaves.FirstOrDefaultAsync(a => a.Matricula == vuelo.Matricula);
       _logger.LogInformation($"Aeronave: {aeronave?.Matricula ?? "NULL"}, Estado: {aeronave?.Estado ?? "NULL"}");
    
   if (aeronave != null && aeronave.Estado == "En Vuelo")
   {
 aeronave.Estado = "En Mantenimiento";
    var disponibleDesde = horaLlegada.AddMinutes(150);
      _logger.LogInformation($"?? Aeronave {aeronave.Matricula} en mantenimiento hasta {disponibleDesde:dd/MM/yyyy HH:mm} (2 horas) - Vuelo {vuelo.NumeroVuelo}");
      }
      }

          var asignacion = await context.AsignacionesEquipoAeronave
         .Include(a => a.Equipo)
.ThenInclude(e => e.EquiposPersonal)
          .ThenInclude(ep => ep.Personal)
 .FirstOrDefaultAsync(a => a.Matricula == vuelo.Matricula && a.Activa);

     _logger.LogInformation($"Asignación: {(asignacion != null ? $"Equipo {asignacion.Equipo.Nombre}, Estado: {asignacion.Equipo.Estado}" : "NULL")}");

     if (asignacion != null && asignacion.Equipo.Estado == "En Servicio")
     {
    var horaFinVuelo = horaLlegada.AddMinutes(30);
var disponibleDesde = horaFinVuelo.AddMinutes(720);

 asignacion.Equipo.Estado = "Descanso";
  asignacion.Equipo.UltimoVueloFin = horaFinVuelo;
        asignacion.Equipo.DisponibleDesde = disponibleDesde;
      _logger.LogInformation($"?? Equipo {asignacion.Equipo.Nombre} en descanso hasta {disponibleDesde:dd/MM/yyyy HH:mm} (12 horas) - Vuelo {vuelo.NumeroVuelo}");

  int personalActualizado = 0;
foreach (var ep in asignacion.Equipo.EquiposPersonal.Where(ep => ep.Activo))
       {
   if (ep.Personal.Estado == "En Servicio")
  {
   ep.Personal.Estado = "Descanso";
        ep.Personal.UltimoVueloFin = horaFinVuelo;
             personalActualizado++;
   _logger.LogInformation($"?? {ep.Personal.Nombre} {ep.Personal.Apellido} en descanso hasta {disponibleDesde:dd/MM/yyyy HH:mm} (12 horas) - Vuelo {vuelo.NumeroVuelo}");
     }
  }
        _logger.LogInformation($"Total personal actualizado a Descanso: {personalActualizado}");
      }
   }
     }
       
     _logger.LogInformation("=== FIN ActualizarEstadosRecursosAsync ===");
  }

        private async Task LiberarRecursosAsync(AppDbContext context, DateTime ahora)
     {
      var aeronavesParaLiberar = await context.Aeronaves
          .Include(a => a.Vuelos)
  .Where(a => a.Estado == "En Mantenimiento")
         .ToListAsync();

          foreach (var aeronave in aeronavesParaLiberar)
            {
     var ultimoVuelo = aeronave.Vuelos
       .Where(v => v.Estado == "Completado")
            .OrderByDescending(v => v.Fecha)
     .ThenByDescending(v => v.HoraLlegada)
      .FirstOrDefault();

if (ultimoVuelo != null)
    {
       var horaLlegada = ultimoVuelo.Fecha.Date.Add(ultimoVuelo.HoraLlegada).AddMinutes(30);
    var disponibleDesde = horaLlegada.AddMinutes(120);

           if (ahora >= disponibleDesde)
    {
  aeronave.Estado = "Operativa";
               _logger.LogInformation($"? Aeronave {aeronave.Matricula} volvió a estado 'Operativa' (después de 2 horas de mantenimiento)");
     }
    }
            }

      var equiposParaLiberar = await context.Equipos
     .Include(e => e.EquiposPersonal)
       .ThenInclude(ep => ep.Personal)
             .Where(e => e.Estado == "Descanso" && e.DisponibleDesde.HasValue && e.DisponibleDesde.Value <= ahora)
     .ToListAsync();

  foreach (var equipo in equiposParaLiberar)
         {
equipo.Estado = "Disponible";
         equipo.DisponibleDesde = null;
           _logger.LogInformation($"? Equipo {equipo.Nombre} volvió a estado 'Disponible' (después de 12 horas de descanso)");

           foreach (var ep in equipo.EquiposPersonal.Where(ep => ep.Activo))
           {
            if (ep.Personal.Estado == "Descanso" && ep.Personal.UltimoVueloFin.HasValue)
         {
          var disponibleDesde = ep.Personal.UltimoVueloFin.Value.AddMinutes(ep.Personal.TiempoDescansoMinutos);
   if (ahora >= disponibleDesde)
       {
      ep.Personal.Estado = "Disponible";
              _logger.LogInformation($"? {ep.Personal.Nombre} {ep.Personal.Apellido} volvió a estado 'Disponible' (después de {ep.Personal.TiempoDescansoMinutos} minutos de descanso)");
          }
         }
    }
            }
      }
    }
}
