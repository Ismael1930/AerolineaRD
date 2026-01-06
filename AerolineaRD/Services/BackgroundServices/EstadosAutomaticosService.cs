using AerolineaRD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AerolineaRD.Services.BackgroundServices
{
    /// <summary>
    /// Servicio en segundo plano que actualiza automáticamente los estados de:
    /// - Aeronaves (Operativa → Mantenimiento después de cada vuelo)
    /// - Equipos (Disponible → Descanso después de cada vuelo)
    /// - Vuelos (Programado → En Curso → Completado según fecha/hora)
    /// </summary>
    public class EstadosAutomaticosService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EstadosAutomaticosService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(5); // Ejecutar cada 5 minutos

        public EstadosAutomaticosService(
      IServiceProvider serviceProvider,
     ILogger<EstadosAutomaticosService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ Servicio de Estados Automáticos iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ActualizarEstadosAsync();
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al actualizar estados automáticos");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("⏹️ Servicio de Estados Automáticos detenido");
        }

        private async Task ActualizarEstadosAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ahora = DateTime.Now;

            _logger.LogDebug($"🔄 Actualizando estados automáticos... ({ahora:dd/MM/yyyy HH:mm})");

            try
            {
                // 1️⃣ Actualizar estados de VUELOS
                await ActualizarEstadosVuelosAsync(context, ahora);

                // 2️⃣ Actualizar estados de AERONAVES (después de vuelos completados)
                await ActualizarEstadosAeronavesAsync(context, ahora);

                // 3️⃣ Actualizar estados de EQUIPOS (después de vuelos completados)
                await ActualizarEstadosEquiposAsync(context, ahora);

                await context.SaveChangesAsync();
                _logger.LogDebug("✅ Estados actualizados correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar estados");
            }
        }

        /// <summary>
        /// Actualiza estados de vuelos según la fecha/hora actual
        /// </summary>
        private async Task ActualizarEstadosVuelosAsync(AppDbContext context, DateTime ahora)
        {
            // 🔹 Vuelos que deben pasar a "En Curso" (hora de salida pasada pero no llegada)
            var vuelosEnCurso = await context.Vuelos
                    .Where(v => v.Estado == "Programado"
          && v.Fecha.Date <= ahora.Date
            && v.HoraSalida <= ahora.TimeOfDay)
                    .ToListAsync();

            foreach (var vuelo in vuelosEnCurso)
            {
                vuelo.Estado = "En Curso";
                _logger.LogInformation($"✈️ Vuelo {vuelo.NumeroVuelo} cambió a 'En Curso'");
            }

            // 🔹 Vuelos que deben pasar a "Completado" (hora de llegada pasada)
            var vuelosCompletados = await context.Vuelos
     .Where(v => (v.Estado == "Programado" || v.Estado == "En Curso")
 && v.Fecha.Date < ahora.Date
   || (v.Fecha.Date == ahora.Date && v.HoraLlegada < ahora.TimeOfDay))
                .ToListAsync();

            foreach (var vuelo in vuelosCompletados)
            {
                vuelo.Estado = "Completado";
                _logger.LogInformation($"✅ Vuelo {vuelo.NumeroVuelo} cambió a 'Completado'");
            }
        }

        /// <summary>
        /// Actualiza estados de aeronaves después de vuelos completados
        /// </summary>
        private async Task ActualizarEstadosAeronavesAsync(AppDbContext context, DateTime ahora)
        {
            // 🔹 Aeronaves que acaban de completar un vuelo → Mantenimiento
            var aeronavesParaMantenimiento = await context.Aeronaves
                   .Include(a => a.Vuelos)
               .Where(a => a.Estado == "Operativa"
                        && a.Vuelos.Any(v => v.Estado == "Completado"
               && v.Fecha.Date == ahora.Date
          && v.HoraLlegada < ahora.TimeOfDay))
          .ToListAsync();

            foreach (var aeronave in aeronavesParaMantenimiento)
            {
                var ultimoVuelo = aeronave.Vuelos
                    .Where(v => v.Estado == "Completado")
            .OrderByDescending(v => v.Fecha)
                    .ThenByDescending(v => v.HoraLlegada)
            .FirstOrDefault();

                if (ultimoVuelo != null)
                {
                    // Tiempo de mantenimiento: por defecto 2 horas (TiempoPreparacionMinutos)
                    var tiempoMantenimiento = aeronave.TiempoPreparacionMinutos > 0
          ? aeronave.TiempoPreparacionMinutos
              : 120;

                    var horaLlegada = ultimoVuelo.Fecha.Date.Add(ultimoVuelo.HoraLlegada);
                    var disponibleDesde = horaLlegada.AddMinutes(tiempoMantenimiento);

                    // Solo poner en mantenimiento si aún no ha pasado el tiempo
                    if (ahora < disponibleDesde)
                    {
                        aeronave.Estado = "En Mantenimiento";
                        _logger.LogInformation(
                      $"🔧 Aeronave {aeronave.Matricula} en mantenimiento hasta {disponibleDesde:dd/MM/yyyy HH:mm}");
                    }
                }
            }

            // 🔹 Aeronaves que terminaron mantenimiento → Operativa
            var aeronavesOperativas = await context.Aeronaves
                .Include(a => a.Vuelos)
     .Where(a => a.Estado == "En Mantenimiento")
       .ToListAsync();

            foreach (var aeronave in aeronavesOperativas)
            {
                var ultimoVuelo = aeronave.Vuelos
                   .Where(v => v.Estado == "Completado")
                 .OrderByDescending(v => v.Fecha)
                             .ThenByDescending(v => v.HoraLlegada)
                                 .FirstOrDefault();

                if (ultimoVuelo != null)
                {
                    var tiempoMantenimiento = aeronave.TiempoPreparacionMinutos > 0
                           ? aeronave.TiempoPreparacionMinutos
                              : 120;

                    var horaLlegada = ultimoVuelo.Fecha.Date.Add(ultimoVuelo.HoraLlegada);
                    var disponibleDesde = horaLlegada.AddMinutes(tiempoMantenimiento);

                    // Si ya pasó el tiempo de mantenimiento, volver a Operativa
                    if (ahora >= disponibleDesde)
                    {
                        aeronave.Estado = "Operativa";
                        _logger.LogInformation($"✅ Aeronave {aeronave.Matricula} volvió a estado 'Operativa'");
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza estados de equipos después de vuelos completados
        /// </summary>
        private async Task ActualizarEstadosEquiposAsync(AppDbContext context, DateTime ahora)
        {
            // 🔹 Equipos que acaban de completar un vuelo → Descanso
            var asignaciones = await context.AsignacionesEquipoAeronave
            .Include(a => a.Equipo)
             .Include(a => a.Aeronave)
          .ThenInclude(aer => aer.Vuelos)
                   .Where(a => a.Activa && a.Equipo.Estado == "En Servicio")
              .ToListAsync();

            foreach (var asignacion in asignaciones)
            {
                var ultimoVuelo = asignacion.Aeronave.Vuelos
                .Where(v => v.Estado == "Completado" && v.Matricula == asignacion.Matricula)
                               .OrderByDescending(v => v.Fecha)
                    .ThenByDescending(v => v.HoraLlegada)
                .FirstOrDefault();

                if (ultimoVuelo != null)
                {
                    var horaLlegada = ultimoVuelo.Fecha.Date.Add(ultimoVuelo.HoraLlegada);

                    // Tiempo de descanso: 8 horas (regulación estándar)
                    var tiempoDescanso = 480; // 8 horas en minutos
                    var disponibleDesde = horaLlegada.AddMinutes(tiempoDescanso);

                    if (ahora < disponibleDesde)
                    {
                        asignacion.Equipo.Estado = "Descanso";
                        asignacion.Equipo.UltimoVueloFin = horaLlegada;
                        asignacion.Equipo.DisponibleDesde = disponibleDesde;

                        _logger.LogInformation(
                        $"💤 Equipo {asignacion.Equipo.Nombre} en descanso hasta {disponibleDesde:dd/MM/yyyy HH:mm}");
                    }
                }
            }

            // 🔹 Equipos que terminaron descanso → Disponible
            var equiposDisponibles = await context.Equipos
                .Where(e => e.Estado == "Descanso" && e.DisponibleDesde.HasValue && e.DisponibleDesde.Value <= ahora)
    .ToListAsync();

            foreach (var equipo in equiposDisponibles)
            {
                equipo.Estado = "Disponible";
                equipo.DisponibleDesde = null;
                _logger.LogInformation($"✅ Equipo {equipo.Nombre} volvió a estado 'Disponible'");
            }
        }
    }
}
