using AerolineaRD.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Asegurar que la base de datos existe
            await context.Database.EnsureCreatedAsync();

            // Crear roles
            await SeedRolesAsync(roleManager);

            // Crear usuarios
            await SeedUsersAsync(userManager);

            // ✅ NUEVO: Actualizar fechas de vuelos existentes si están en el pasado
            await ActualizarFechasVuelosAsync(context);

            // Si ya hay datos, no hacer nada más
            if (await context.Aeropuertos.AnyAsync())
            {
                return;
            }

            // 1. AEROPUERTOS
            await SeedAeropuertosAsync(context);

            // 2. AERONAVES
            await SeedAeronavesAsync(context);

            // 3. TRIPULACIÓN
            await SeedTripulacionAsync(context);

            // ✅ NUEVO: 3.5 PERSONAL Y EQUIPOS (Sistema Nuevo)
            await SeedPersonalYEquiposAsync(context);

            // 4. VUELOS
            await SeedVuelosAsync(context);

            // 5. ASIENTOS
            await SeedAsientosAsync(context);

            // 6. CLIENTES
            await SeedClientesAsync(context, userManager);

            // 7. PASAJEROS
            await SeedPasajerosAsync(context);

            // 8. RESERVAS
            await SeedReservasAsync(context);

            // 9. FACTURAS
            await SeedFacturasAsync(context);

            // 10. EQUIPAJES
            await SeedEquipajesAsync(context);

            // 11. ESTADOS DE VUELO
            await SeedEstadosVueloAsync(context);

            // 12. ASIGNACIÓN TRIPULACIÓN-VUELO
            await SeedVueloTripulacionAsync(context);

            // 13. NOTIFICACIONES
            await SeedNotificacionesAsync(context);

            Console.WriteLine("✅ Seeder completado exitosamente!");
            Console.WriteLine($"   - {await context.Aeropuertos.CountAsync()} aeropuertos");
            Console.WriteLine($"   - {await context.Aeronaves.CountAsync()} aeronaves");
            Console.WriteLine($"   - {await context.Tripulaciones.CountAsync()} tripulantes");
            Console.WriteLine($"   - {await context.Vuelos.CountAsync()} vuelos");
            Console.WriteLine($"   - {await context.Asientos.CountAsync()} asientos");
            Console.WriteLine($"   - {await context.Clientes.CountAsync()} clientes");
            Console.WriteLine($"   - {await context.Pasajeros.CountAsync()} pasajeros");
            Console.WriteLine($" - {await context.Reservas.CountAsync()} reservas");
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Cliente", "Empleado" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            // Admin
            if (await userManager.FindByEmailAsync("admin@aerolineard.com") == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin@aerolineard.com",
                    Email = "admin@aerolineard.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Cliente de prueba
            if (await userManager.FindByEmailAsync("cliente@test.com") == null)
            {
                var clienteUser = new IdentityUser
                {
                    UserName = "cliente@test.com",
                    Email = "cliente@test.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(clienteUser, "Cliente123!");
                await userManager.AddToRoleAsync(clienteUser, "Cliente");
            }
        }

        private static async Task SeedAeropuertosAsync(AppDbContext context)
        {
            var aeropuertos = new List<Aeropuerto>
    {
         // ✅ AEROPUERTO DE PRUEBA - Para demostrar validaciones
    new Aeropuerto { Codigo = "TEST", Nombre = "Aeropuerto de Prueba Validaciones", Ciudad = "Test City", Pais = "República Dominicana", CapacidadVuelosPorHora = 1 },
            
       // República Dominicana
              new Aeropuerto { Codigo = "SDQ", Nombre = "Las Américas", Ciudad = "Santo Domingo", Pais = "República Dominicana", CapacidadVuelosPorHora = 15 },
        new Aeropuerto { Codigo = "PUJ", Nombre = "Punta Cana", Ciudad = "Punta Cana", Pais = "República Dominicana", CapacidadVuelosPorHora = 12 },
  new Aeropuerto { Codigo = "STI", Nombre = "Cibao", Ciudad = "Santiago", Pais = "República Dominicana", CapacidadVuelosPorHora = 10 },
      new Aeropuerto { Codigo = "POP", Nombre = "Gregorio Luperón", Ciudad = "Puerto Plata", Pais = "República Dominicana", CapacidadVuelosPorHora = 8 },
     new Aeropuerto { Codigo = "LRM", Nombre = "La Romana", Ciudad = "La Romana", Pais = "República Dominicana", CapacidadVuelosPorHora = 6 },
          
     // Estados Unidos
 new Aeropuerto { Codigo = "JFK", Nombre = "John F. Kennedy", Ciudad = "Nueva York", Pais = "Estados Unidos", CapacidadVuelosPorHora = 30 },
                new Aeropuerto { Codigo = "MIA", Nombre = "Miami International", Ciudad = "Miami", Pais = "Estados Unidos", CapacidadVuelosPorHora = 25 },
    new Aeropuerto { Codigo = "EWR", Nombre = "Newark Liberty", Ciudad = "Newark", Pais = "Estados Unidos", CapacidadVuelosPorHora = 28 },
  new Aeropuerto { Codigo = "FLL", Nombre = "Fort Lauderdale", Ciudad = "Fort Lauderdale", Pais = "Estados Unidos", CapacidadVuelosPorHora = 20 },
     new Aeropuerto { Codigo = "ATL", Nombre = "Hartsfield-Jackson", Ciudad = "Atlanta", Pais = "Estados Unidos", CapacidadVuelosPorHora = 35 },

                // Europa
      new Aeropuerto { Codigo = "MAD", Nombre = "Adolfo Suárez", Ciudad = "Madrid", Pais = "España", CapacidadVuelosPorHora = 30 },
   new Aeropuerto { Codigo = "BCN", Nombre = "El Prat", Ciudad = "Barcelona", Pais = "España", CapacidadVuelosPorHora = 25 },
     new Aeropuerto { Codigo = "CDG", Nombre = "Charles de Gaulle", Ciudad = "París", Pais = "Francia", CapacidadVuelosPorHora = 32 },
        
        // Latinoamérica
 new Aeropuerto { Codigo = "CUN", Nombre = "Cancún", Ciudad = "Cancún", Pais = "México", CapacidadVuelosPorHora = 18 },
    new Aeropuerto { Codigo = "PTY", Nombre = "Tocumen", Ciudad = "Panamá", Pais = "Panamá", CapacidadVuelosPorHora = 15 },
    new Aeropuerto { Codigo = "BOG", Nombre = "El Dorado", Ciudad = "Bogotá", Pais = "Colombia", CapacidadVuelosPorHora = 22 },
     new Aeropuerto { Codigo = "LIM", Nombre = "Jorge Chávez", Ciudad = "Lima", Pais = "Perú", CapacidadVuelosPorHora = 20 }
    };

            await context.Aeropuertos.AddRangeAsync(aeropuertos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAeronavesAsync(AppDbContext context)
        {
            var aeronaves = new List<Aeronave>
              {
          new Aeronave { Matricula = "HI-1001RD", Modelo = "Boeing 737-800", Capacidad = 189, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
      new Aeronave { Matricula = "HI-1002RD", Modelo = "Boeing 737-800", Capacidad = 189, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
            new Aeronave { Matricula = "HI-1003RD", Modelo = "Airbus A320", Capacidad = 180, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
              new Aeronave { Matricula = "HI-1004RD", Modelo = "Airbus A320", Capacidad = 180, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
        new Aeronave { Matricula = "HI-1005RD", Modelo = "Boeing 787-9", Capacidad = 296, Estado = "Operativa", TiempoPreparacionMinutos = 180 },
                    new Aeronave { Matricula = "HI-1006RD", Modelo = "Airbus A321", Capacidad = 220, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
                    new Aeronave { Matricula = "HI-1007RD", Modelo = "Boeing 737-MAX", Capacidad = 178, Estado = "Operativa", TiempoPreparacionMinutos = 120 },
              new Aeronave { Matricula = "HI-1008RD", Modelo = "Embraer E195", Capacidad = 132, Estado = "Mantenimiento", TiempoPreparacionMinutos = 90 }
         };

            await context.Aeronaves.AddRangeAsync(aeronaves);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTripulacionAsync(AppDbContext context)
        {
            var tripulacion = new List<Tripulacion>
            {
   // Pilotos
    new Tripulacion { Nombre = "Carlos", Apellido = "Rodríguez", Rol = "Piloto", Licencia = "ATP-001", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Boeing 737,Boeing 737-800,Boeing 737-MAX" },
  new Tripulacion { Nombre = "María", Apellido = "Santos", Rol = "Piloto", Licencia = "ATP-002", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Airbus A320,Airbus A321" },
  new Tripulacion { Nombre = "Juan", Apellido = "Pérez", Rol = "Piloto", Licencia = "ATP-003", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Boeing 787,Boeing 737" },
           new Tripulacion { Nombre = "Ana", Apellido = "Martínez", Rol = "Piloto", Licencia = "ATP-004", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Airbus A320,Airbus A321,Boeing 737" },
            
    // Copilotos
            new Tripulacion { Nombre = "Luis", Apellido = "García", Rol = "Copiloto", Licencia = "CPL-001", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Boeing 737,Boeing 737-800" },
     new Tripulacion { Nombre = "Carmen", Apellido = "López", Rol = "Copiloto", Licencia = "CPL-002", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Airbus A320,Airbus A321" },
     new Tripulacion { Nombre = "Pedro", Apellido = "Hernández", Rol = "Copiloto", Licencia = "CPL-003", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Boeing 787,Boeing 737-MAX" },
             new Tripulacion { Nombre = "Isabel", Apellido = "Gómez", Rol = "Copiloto", Licencia = "CPL-004", TiempoDescansoMinutos = 480, CertificacionesAeronave = "Embraer E195,Airbus A320" },
  
                // Sobrecargos
           new Tripulacion { Nombre = "Rosa", Apellido = "Díaz", Rol = "Sobrecargo Jefe", Licencia = "FA-001", TiempoDescansoMinutos = 480 },
             new Tripulacion { Nombre = "Miguel", Apellido = "Torres", Rol = "Sobrecargo", Licencia = "FA-002", TiempoDescansoMinutos = 480 },
     new Tripulacion { Nombre = "Laura", Apellido = "Ramírez", Rol = "Sobrecargo", Licencia = "FA-003", TiempoDescansoMinutos = 480 },
    new Tripulacion { Nombre = "José", Apellido = "Flores", Rol = "Sobrecargo", Licencia = "FA-004", TiempoDescansoMinutos = 480 },
    new Tripulacion { Nombre = "Patricia", Apellido = "Morales", Rol = "Sobrecargo", Licencia = "FA-005", TiempoDescansoMinutos = 480 },
  new Tripulacion { Nombre = "Roberto", Apellido = "Cruz", Rol = "Sobrecargo", Licencia = "FA-006", TiempoDescansoMinutos = 480 }
          };

            await context.Tripulaciones.AddRangeAsync(tripulacion);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// ✅ NUEVO: Seed completo de Personal, Equipos y Asignaciones
        /// Demuestra validaciones de equipos, personal y asignación a aeronaves
        /// </summary>
        private static async Task SeedPersonalYEquiposAsync(AppDbContext context)
        {
            // Si ya existen datos, no duplicar
            if (await context.Personal.AnyAsync())
            {
                Console.WriteLine("⏭️  Personal ya existe, saltando seed de equipos");
                return;
            }

            // ========== 1. CREAR PERSONAL ==========
            var personal = new List<Personal>();

            // ✅ Pilotos (4 pilotos para 2 equipos)
            personal.AddRange(new[]
              {
       new Personal
  {
    Nombre = "Capitán Roberto",
 Apellido = "Fernández",
  Rol = "Piloto",
Licencia = "ATP-P001",
     CertificacionesAeronave = "Boeing 737,Boeing 737-800,Airbus A320",
        TiempoDescansoMinutos = 480,
Estado = "Disponible",
       FechaContratacion = DateTime.Today.AddYears(-5),
        Activo = true
  },
      new Personal
  {
       Nombre = "Capitán Daniel",
      Apellido = "Moreno",
Rol = "Piloto",
        Licencia = "ATP-P002",
      CertificacionesAeronave = "Boeing 737,Boeing 787-9,Airbus A321",
      TiempoDescansoMinutos = 480,
Estado = "Disponible",
FechaContratacion = DateTime.Today.AddYears(-4),
     Activo = true
},
   new Personal
  {
    Nombre = "Capitana Sofía",
Apellido = "Ramírez",
       Rol = "Piloto",
   Licencia = "ATP-P003",
CertificacionesAeronave = "Airbus A320,Airbus A321,Boeing 737-MAX",
    TiempoDescansoMinutos = 480,
   Estado = "Disponible",
       FechaContratacion = DateTime.Today.AddYears(-6),
  Activo = true
 },
    new Personal
 {
      Nombre = "Capitán Miguel",
    Apellido = "Vargas",
  Rol = "Piloto",
 Licencia = "ATP-P004",
CertificacionesAeronave = "Boeing 787-9,Embraer E195",
      TiempoDescansoMinutos = 480,
   Estado = "Disponible",
        FechaContratacion = DateTime.Today.AddYears(-3),
    Activo = true
 }
   });

            // ✅ Copilotos (4 copilotos para 2 equipos)
            personal.AddRange(new[]
                    {
      new Personal
     {
      Nombre = "Primer Oficial Carlos",
   Apellido = "Jiménez",
Rol = "Copiloto",
      Licencia = "CPL-C001",
 CertificacionesAeronave = "Boeing 737,Boeing 737-800",
       TiempoDescansoMinutos = 480,
   Estado = "Disponible",
       FechaContratacion = DateTime.Today.AddYears(-3),
       Activo = true
 },
      new Personal
     {
       Nombre = "Primer Oficial Andrea",
Apellido = "Castillo",
      Rol = "Copiloto",
   Licencia = "CPL-C002",
        CertificacionesAeronave = "Airbus A320,Airbus A321",
   TiempoDescansoMinutos = 480,
       Estado = "Disponible",
       FechaContratacion = DateTime.Today.AddYears(-2),
Activo = true
  },
       new Personal
{
   Nombre = "Primer Oficial Jorge",
Apellido = "Silva",
      Rol = "Copiloto",
Licencia = "CPL-C003",
       CertificacionesAeronave = "Boeing 787-9,Boeing 737-MAX",
TiempoDescansoMinutos = 480,
        Estado = "Disponible",
 FechaContratacion = DateTime.Today.AddYears(-4),
    Activo = true
 },
      new Personal
     {
     Nombre = "Primer Oficial Valentina",
Apellido = "Ortiz",
      Rol = "Copiloto",
Licencia = "CPL-C004",
    CertificacionesAeronave = "Embraer E195,Airbus A320",
  TiempoDescansoMinutos = 480,
       Estado = "Disponible",
FechaContratacion = DateTime.Today.AddYears(-2),
    Activo = true
   }
   });

            // ✅ Sobrecargos Jefe (2 para cada equipo)
            personal.AddRange(new[]
           {
new Personal
     {
     Nombre = "Jefe de Cabina Elena",
       Apellido = "Rojas",
      Rol = "Sobrecargo Jefe",
Licencia = "FA-J001",
     TiempoDescansoMinutos = 480,
  Estado = "Disponible",
   FechaContratacion = DateTime.Today.AddYears(-7),
Activo = true
     },
     new Personal
     {
      Nombre = "Jefe de Cabina Fernando",
      Apellido = "Mendoza",
    Rol = "Sobrecargo Jefe",
        Licencia = "FA-J002",
TiempoDescansoMinutos = 480,
 Estado = "Disponible",
    FechaContratacion = DateTime.Today.AddYears(-6),
 Activo = true
     }
   });

            // ✅ Sobrecargos (12 para distribuir entre 2 equipos: 6 c/u, cumplir 3-6 requeridos)
            personal.AddRange(new[]
      {
      new Personal { Nombre = "Sobrecargo Lucía", Apellido = "Pérez", Rol = "Sobrecargo", Licencia = "FA-S001", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-5), Activo = true },
    new Personal { Nombre = "Sobrecargo Marcos", Apellido = "Reyes", Rol = "Sobrecargo", Licencia = "FA-S002", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-4), Activo = true },
 new Personal { Nombre = "Sobrecargo Diana", Apellido = "Herrera", Rol = "Sobrecargo", Licencia = "FA-S003", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-3), Activo = true },
   new Personal { Nombre = "Sobrecargo Esteban", Apellido = "Gutiérrez", Rol = "Sobrecargo", Licencia = "FA-S004", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-3), Activo = true },
      new Personal { Nombre = "Sobrecargo Camila", Apellido = "Navarro", Rol = "Sobrecargo", Licencia = "FA-S005", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
 new Personal { Nombre = "Sobrecargo Antonio", Apellido = "Ríos", Rol = "Sobrecargo", Licencia = "FA-S006", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
      new Personal { Nombre = "Sobrecargo Natalia", Apellido = "Molina", Rol = "Sobrecargo", Licencia = "FA-S007", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-4), Activo = true },
        new Personal { Nombre = "Sobrecargo Rodrigo", Apellido = "Vega", Rol = "Sobrecargo", Licencia = "FA-S008", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-3), Activo = true },
new Personal { Nombre = "Sobrecargo Gabriela", Apellido = "Luna", Rol = "Sobrecargo", Licencia = "FA-S009", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
      new Personal { Nombre = "Sobrecargo Manuel", Apellido = "Cruz", Rol = "Sobrecargo", Licencia = "FA-S010", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-5), Activo = true },
    new Personal { Nombre = "Sobrecargo Victoria", Apellido = "Pardo", Rol = "Sobrecargo", Licencia = "FA-S011", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-1), Activo = true },
    new Personal { Nombre = "Sobrecargo Andrés", Apellido = "Campos", Rol = "Sobrecargo", Licencia = "FA-S012", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-1), Activo = true }
  });

            await context.Personal.AddRangeAsync(personal);
            await context.SaveChangesAsync();

            // ========== 2. CREAR EQUIPOS COMPLETOS ==========
            var equipos = new List<Equipo>
        {
       new Equipo
 {
   Nombre = "Equipo Alpha",
      Codigo = "ALPHA-01",
     Estado = "Disponible",
     FechaCreacion = DateTime.Today.AddMonths(-6),
Activo = true
      },
    new Equipo
      {
Nombre = "Equipo Bravo",
   Codigo = "BRAVO-01",
     Estado = "Disponible",
        FechaCreacion = DateTime.Today.AddMonths(-5),
        Activo = true
  },
    new Equipo
  {
Nombre = "Equipo Charlie",
  Codigo = "CHARLIE-01",
   Estado = "Incompleto", // ✅ Este NO tiene miembros asignados (para demostrar validación)
       FechaCreacion = DateTime.Today.AddMonths(-1),
        Activo = true
  }
        };

            await context.Equipos.AddRangeAsync(equipos);
            await context.SaveChangesAsync();

            // ========== 3. ASIGNAR PERSONAL A EQUIPOS ==========
            var equiposPersonal = new List<EquipoPersonal>();

            // ✅ EQUIPO ALPHA (Completo y válido)
            var equipoAlpha = equipos[0];
            equiposPersonal.AddRange(new[]
              {
     new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[0].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Piloto 1
       new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[4].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Copiloto 1
   new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[8].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Sobrecargo Jefe 1
 new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[10].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Sobrecargo 1
       new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[11].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Sobrecargo 2
 new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[12].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }, // Sobrecargo 3
      new EquipoPersonal { IdEquipo = equipoAlpha.Id, IdPersonal = personal[13].Id, FechaAsignacion = DateTime.Today.AddMonths(-6), Activo = true }  // Sobrecargo 4
   });

            // ✅ EQUIPO BRAVO (Completo y válido)
            var equipoBravo = equipos[1];
            equiposPersonal.AddRange(new[]
        {
     new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[1].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Piloto 2
      new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[5].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Copiloto 2
       new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[9].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Sobrecargo Jefe 2
new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[14].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Sobrecargo 5
   new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[15].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Sobrecargo 6
        new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[16].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Sobrecargo 7
      new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[17].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }, // Sobrecargo 8
   new EquipoPersonal { IdEquipo = equipoBravo.Id, IdPersonal = personal[18].Id, FechaAsignacion = DateTime.Today.AddMonths(-5), Activo = true }  // Sobrecargo 9
        });

            // ❌ EQUIPO CHARLIE no tiene miembros (para demostrar validación de equipo incompleto)

            await context.EquipoPersonal.AddRangeAsync(equiposPersonal);
            await context.SaveChangesAsync();

            // ========== 4. ASIGNAR EQUIPOS A AERONAVES ==========
            var asignaciones = new List<AsignacionEquipoAeronave>
     {
    new AsignacionEquipoAeronave
      {
IdEquipo = equipoAlpha.Id,
    Matricula = "HI-1001RD",
       FechaAsignacion = DateTime.Today.AddMonths(-6),
      Activa = true,
Observaciones = "Asignación inicial - Equipo Alpha certificado en Boeing 737-800"
        },
new AsignacionEquipoAeronave
{
          IdEquipo = equipoBravo.Id,
       Matricula = "HI-1002RD",
FechaAsignacion = DateTime.Today.AddMonths(-5),
  Activa = true,
         Observaciones = "Asignación inicial - Equipo Bravo certificado en Boeing 737-800"
}
  };

            await context.AsignacionesEquipoAeronave.AddRangeAsync(asignaciones);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ Seed de Personal y Equipos completado:");
            Console.WriteLine($"      - {personal.Count} miembros de personal creados");
            Console.WriteLine($"      - {equipos.Count} equipos creados (2 completos, 1 incompleto)");
            Console.WriteLine($"      - {equiposPersonal.Count} asignaciones personal-equipo");
            Console.WriteLine($"      - {asignaciones.Count} equipos asignados a aeronaves");
            Console.WriteLine($"      - HI-1001RD → Equipo Alpha");
            Console.WriteLine($"  - HI-1002RD → Equipo Bravo");
            Console.WriteLine($"      - HI-1003RD, HI-1004RD, HI-1005RD, HI-1006RD, HI-1007RD → SIN EQUIPO (para validaciones)");
        }
        private static async Task SeedVuelosAsync(AppDbContext context)
        {
     // ✅ FECHA BASE: Siempre mañana
            var fechaBase = DateTime.Today.AddDays(1);
       var hoy = DateTime.Today;

            var vuelos = new List<Vuelo>();

            // ✅ AEROPUERTO DE PRUEBA
            var aeropuertoPrueba = await context.Aeropuertos.FirstOrDefaultAsync(a => a.Codigo == "TEST");
      if (aeropuertoPrueba == null)
            {
            aeropuertoPrueba = new Aeropuerto
     {
                    Codigo = "TEST",
      Nombre = "Aeropuerto de Prueba Validaciones",
    Ciudad = "Test City",
      Pais = "República Dominicana",
            CapacidadVuelosPorHora = 1
    };
      await context.Aeropuertos.AddAsync(aeropuertoPrueba);
        await context.SaveChangesAsync();
      }

       // ✅ VUELO DE PRUEBA PARA HOY - 9:00 AM - 9:30 AM
       // Este vuelo servirá para probar el sistema de estados automáticos
       vuelos.Add(new Vuelo
 {
             NumeroVuelo = "TEST-LIVE",
        Fecha = hoy, // ✅ HOY
 HoraSalida = new TimeSpan(9, 0, 0), // ✅ 9:00 AM (09:00)
    HoraLlegada = new TimeSpan(9, 30, 0), // ✅ 9:30 AM (09:30)
      Duracion = 30,
 PrecioBase = 150.00m,
      OrigenCodigo = "SDQ",
       DestinoCodigo = "PUJ",
   Matricula = "HI-1001RD", // ✅ Aeronave con Equipo Alpha asignado
 Estado = "Programado",
  TipoVuelo = "SoloIda",
     ClasesDisponibles = "Economica,Ejecutiva,Primera"
      });

            // ✅ Crear vuelos en el aeropuerto TEST (mañana) - HI-1001RD
            // Distribución: 6 AM a 8 PM (evitando solapamiento con TEST-LIVE de hoy a las 9 PM)
       int numeroVuelo = 2000;
            for (int hora = 6; hora <= 20; hora += 2) // Vuelos cada 2 horas
         {
   vuelos.Add(new Vuelo
         {
          NumeroVuelo = $"TEST{numeroVuelo++}",
        Fecha = fechaBase,
      HoraSalida = new TimeSpan(hora, 0, 0),
       HoraLlegada = new TimeSpan(hora + 1, 30, 0),
       Duracion = 90,
          PrecioBase = 200.00m,
         OrigenCodigo = "TEST",
              DestinoCodigo = "SDQ",
         Matricula = "HI-1001RD",
        Estado = "Programado",
    TipoVuelo = "SoloIda",
    ClasesDisponibles = "Economica,Ejecutiva,Primera"
                });
            }

         // ✅ Crear más vuelos TEST para el día siguiente - HI-1001RD
            for (int hora = 6; hora <= 18; hora += 2)
   {
   vuelos.Add(new Vuelo
           {
          NumeroVuelo = $"TEST{numeroVuelo++}",
       Fecha = fechaBase.AddDays(1),
 HoraSalida = new TimeSpan(hora, 0, 0),
        HoraLlegada = new TimeSpan(hora + 1, 30, 0),
       Duracion = 90,
      PrecioBase = 200.00m,
          OrigenCodigo = "TEST",
       DestinoCodigo = "SDQ",
          Matricula = "HI-1001RD",
   Estado = "Programado",
         TipoVuelo = "SoloIda",
        ClasesDisponibles = "Economica,Ejecutiva,Primera"
       });
            }

         // ✅ AEROPUERTO CIBAO: Crear 9 vuelos - HI-1002RD (sin solapamiento)
            numeroVuelo = 3000;
     for (int i = 0; i < 9; i++)
 {
           vuelos.Add(new Vuelo
     {
     NumeroVuelo = $"STI{numeroVuelo++}",
  Fecha = fechaBase,
  HoraSalida = new TimeSpan(8 + i, 0, 0),
        HoraLlegada = new TimeSpan(10 + i, 30, 0),
              Duracion = 150,
        PrecioBase = 320.00m,
      OrigenCodigo = "STI",
        DestinoCodigo = "MIA",
       Matricula = "HI-1002RD",
    Estado = "Programado",
    TipoVuelo = "SoloIda",
   ClasesDisponibles = "Economica,Ejecutiva,Primera"
        });
            }

            // ✅ Vuelos variados con diferentes aeronaves (sin solapamiento)
            numeroVuelo = 1000;
  var rutasVariadas = new[]
       {
             (origen: "SDQ", destino: "JFK", precio: 450.00m, duracion: 255, matricula: "HI-1003RD", hora: 10),
     (origen: "SDQ", destino: "MIA", precio: 320.00m, duracion: 150, matricula: "HI-1004RD", hora: 11),
                (origen: "PUJ", destino: "ATL", precio: 400.00m, duracion: 185, matricula: "HI-1005RD", hora: 12),
                (origen: "PUJ", destino: "JFK", precio: 480.00m, duracion: 270, matricula: "HI-1006RD", hora: 13),
   (origen: "SDQ", destino: "MAD", precio: 850.00m, duracion: 540, matricula: "HI-1007RD", hora: 14)
            };

          for (int i = 0; i < rutasVariadas.Length; i++)
     {
  var ruta = rutasVariadas[i];

        // Vuelos de ida y vuelta
          vuelos.Add(new Vuelo
             {
NumeroVuelo = $"RD{numeroVuelo++}",
      Fecha = fechaBase.AddDays(i + 2),
       HoraSalida = new TimeSpan(ruta.hora, 0, 0),
                    HoraLlegada = new TimeSpan(ruta.hora, 0, 0).Add(TimeSpan.FromMinutes(ruta.duracion)),
             Duracion = ruta.duracion,
      PrecioBase = ruta.precio,
    OrigenCodigo = ruta.origen,
             DestinoCodigo = ruta.destino,
    Matricula = ruta.matricula,
  Estado = "Programado",
          TipoVuelo = "IdaYVuelta",
        FechaRegreso = fechaBase.AddDays(i + 9),
      ClasesDisponibles = "Economica,Ejecutiva,Primera"
        });

       // Vuelos de solo ida (diferentes horarios para evitar solapamiento)
       vuelos.Add(new Vuelo
       {
         NumeroVuelo = $"RD{numeroVuelo++}",
                    Fecha = fechaBase.AddDays(i + 10),
        HoraSalida = new TimeSpan(ruta.hora + 5, 0, 0), // 5 horas después
  HoraLlegada = new TimeSpan(ruta.hora + 5, 0, 0).Add(TimeSpan.FromMinutes(ruta.duracion)),
           Duracion = ruta.duracion,
   PrecioBase = ruta.precio,
       OrigenCodigo = ruta.origen,
     DestinoCodigo = ruta.destino,
          Matricula = ruta.matricula,
        Estado = "Programado",
      TipoVuelo = "SoloIda",
   ClasesDisponibles = "Economica,Ejecutiva,Primera"
       });
          }

            await context.Vuelos.AddRangeAsync(vuelos);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ Creados {vuelos.Count} vuelos de PRUEBA:");
            Console.WriteLine($"      - 1 vuelo LIVE HOY (TEST-LIVE) - HOY 9:00 AM - 9:30 AM (30 min) - ¡Prueba de estados automáticos!");
            Console.WriteLine($"      - {rutasVariadas.Length * 2} vuelos variados con diferentes aeronaves");
    Console.WriteLine($"      - Vuelos TEST en aeronave HI-1001RD: {vuelos.Count(v => v.Matricula == "HI-1001RD")}");
   Console.WriteLine($"      - Vuelos STI en aeronave HI-1002RD: {vuelos.Count(v => v.Matricula == "HI-1002RD")}");
   Console.WriteLine($"      - Fecha base: {fechaBase:dd/MM/yyyy}");
   Console.WriteLine($"      ⚠️  IMPORTANTE: Sin solapamiento de horarios por aeronave");
        }

        private static async Task SeedAsientosAsync(AppDbContext context)
        {
            // ⬅️ CAMBIO: Ahora los asientos pertenecen a AERONAVES, no a VUELOS
            var aeronaves = await context.Aeronaves.ToListAsync();
            var asientos = new List<Asiento>();

            foreach (var aeronave in aeronaves)
            {
                // Primera Clase (Filas 1-3): 12 asientos
                for (int fila = 1; fila <= 3; fila++)
                {
                    foreach (var letra in new[] { "A", "B", "C", "D" })
                    {
                        asientos.Add(new Asiento
                        {
                            Numero = $"{aeronave.Matricula}-{fila}{letra}", // Ej: "HI1001RD-1A"
                            Matricula = aeronave.Matricula,
                            NumeroAsiento = $"{fila}{letra}", // Ej: "1A"
                            Clase = "Primera"
                        });
                    }
                }

                // Clase Ejecutiva (Filas 4-8): 20 asientos
                for (int fila = 4; fila <= 8; fila++)
                {
                    foreach (var letra in new[] { "A", "B", "C", "D" })
                    {
                        asientos.Add(new Asiento
                        {
                            Numero = $"{aeronave.Matricula}-{fila}{letra}",
                            Matricula = aeronave.Matricula,
                            NumeroAsiento = $"{fila}{letra}",
                            Clase = "Ejecutiva"
                        });
                    }
                }

                // Clase Económica (Filas 9-30): 132 asientos
                for (int fila = 9; fila <= 30; fila++)
                {
                    foreach (var letra in new[] { "A", "B", "C", "D", "E", "F" })
                    {
                        asientos.Add(new Asiento
                        {
                            Numero = $"{aeronave.Matricula}-{fila}{letra}",
                            Matricula = aeronave.Matricula,
                            NumeroAsiento = $"{fila}{letra}",
                            Clase = "Economica"
                        });
                    }
                }
            }

            await context.Asientos.AddRangeAsync(asientos);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ Creados {asientos.Count} asientos para {aeronaves.Count} aeronaves");
            Console.WriteLine($"      - Por aeronave: 164 asientos (12 Primera + 20 Ejecutiva + 132 Económica)");
        }

        private static async Task SeedClientesAsync(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            var clienteUser = await userManager.FindByEmailAsync("cliente@test.com");

            var clientes = new List<Cliente>();

            // Crear cliente vinculado al usuario si no existe
            if (clienteUser != null && !await context.Clientes.AnyAsync(c => c.Email == clienteUser.Email))
            {
                clientes.Add(new Cliente
                {
                    Nombre = "Juan Cliente",
                    Email = clienteUser.Email,
                    Telefono = "+1809-555-0001",
                    UserId = clienteUser.Id
                });
            }

            // Clientes adicionales (solo se crean si no existen ya)
            var adicionales = new[]
            {
                new Cliente { Nombre = "María González", Email = "maria.gonzalez@example.com", Telefono = "+1809-555-0002" },
                new Cliente { Nombre = "Pedro Sánchez", Email = "pedro.sanchez@example.com", Telefono = "+1809-555-0003" }
            };

            foreach (var c in adicionales)
            {
                if (!await context.Clientes.AnyAsync(x => x.Email == c.Email))
                    clientes.Add(c);
            }

            if (clientes.Any())
            {
                await context.Clientes.AddRangeAsync(clientes);
                await context.SaveChangesAsync();
            }
        }

        private static string GeneratePasaporte(Random rnd)
        {
            // Formato: P +8 dígitos
            return "P" + rnd.Next(10_000_000, 99_999_999).ToString();
        }

        private static async Task SeedPasajerosAsync(AppDbContext context)
        {
            var rnd = new Random();

            // Evitar duplicar si ya existen pasajeros
            if (await context.Pasajeros.AnyAsync())
                return;

            var pasajeros = new List<Pasajero>();

            // Obtener clientes existentes para vincular algunos pasajeros
            var clientes = await context.Clientes.ToListAsync();

            // Crear pasajeros, asignando IdCliente cuando sea posible
            var nombres = new[] { "Juan", "María", "Pedro", "Ana", "Luis" };
            var apellidos = new[] { "Cliente", "González", "Sánchez", "Martínez", "Rodríguez" };

            for (int i = 0; i < nombres.Length; i++)
            {
                string pasaporte;
                // Generar pasaporte único
                do
                {
                    pasaporte = GeneratePasaporte(rnd);
                } while (await context.Pasajeros.AnyAsync(p => p.Pasaporte == pasaporte));

                var pasajero = new Pasajero
                {
                    Nombre = nombres[i],
                    Apellido = apellidos[i],
                    Pasaporte = pasaporte,
                    IdCliente = clientes.Count > i ?clientes[i].Id : (int?)null
                };
                pasajeros.Add(pasajero);
            }

            await context.Pasajeros.AddRangeAsync(pasajeros);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReservasAsync(AppDbContext context)
        {
            // ⚠️ RESERVAS COMENTADAS - Dependen de vuelos que no existen
            Console.WriteLine("⚠️  Creación de reservas deshabilitada - Sin vuelos para reservar");
            return;

            /* ========== CÓDIGO COMENTADO ==========
var vuelos = await context.Vuelos
        .Include(v => v.Aeronave)
        .ThenInclude(a => a.Asientos)
    .Take(5)
     .ToListAsync();

     var clientes = await context.Clientes.ToListAsync();
            var pasajeros = await context.Pasajeros.ToListAsync();

            if (!vuelos.Any() || !clientes.Any() || !pasajeros.Any())
       return;

      var reservas = new List<Reserva>();

            // Para cada vuelo, tomar un asiento económico de su aeronave
   for (int i = 0; i < Math.Min(3, vuelos.Count); i++)
            {
     var vuelo = vuelos[i];
     var asientoDisponible = vuelo.Aeronave?.Asientos?
          .FirstOrDefault(a => a.Clase == "Economica");

      if (asientoDisponible == null) continue;

      reservas.Add(new Reserva
         {
    Codigo = $"RES{(i + 1):000}",
    IdVuelo = vuelo.Id,
     IdCliente = clientes[i % clientes.Count].Id,
       IdPasajero = pasajeros[i % pasajeros.Count].Id,
        NumAsiento = asientoDisponible.NumeroAsiento,
             Clase = "Economica",
        FechaReserva = DateTime.Today.AddDays(-(5 - i)),
  Estado = "Confirmada",
         PrecioTotal = vuelo.PrecioBase
 });
   }

       await context.Reservas.AddRangeAsync(reservas);
            await context.SaveChangesAsync();
            ========== FIN CÓDIGO COMENTADO ========== */
        }
        private static async Task SeedFacturasAsync(AppDbContext context)
        {
            // ⚠️ FACTURAS COMENTADAS - Dependen de reservas que no existen
            Console.WriteLine("⚠️  Creación de facturas deshabilitada - Sin reservas para facturar");
            return;

            /* ========== CÓDIGO COMENTADO ==========
             var reservas = await context.Reservas.ToListAsync();

              if (!reservas.Any())
                return;

            var facturas = new List<Factura>
                    {
            new Factura
            {
                Codigo = "FAC001",
                CodReserva = reservas[0].Codigo,
                 Monto = reservas[0].PrecioTotal,
             MetodoPago = "Tarjeta de Crédito",
               FechaEmision = reservas[0].FechaReserva,
               EstadoPago = "Pagado"
         },
                  new Factura
            {
           Codigo = "FAC002",
                 CodReserva = reservas[1].Codigo,
              Monto = reservas[1].PrecioTotal,
            MetodoPago = "PayPal",
                FechaEmision = reservas[1].FechaReserva,
              EstadoPago = "Pagado"
          }
          };

               await context.Facturas.AddRangeAsync(facturas);
                await context.SaveChangesAsync();
             ========== FIN CÓDIGO COMENTADO ========== */
        }

        private static async Task SeedEquipajesAsync(AppDbContext context)
        {
            var pasajeros = await context.Pasajeros.Take(3).ToListAsync();

            if (!pasajeros.Any())
                return;

            var equipajes = new List<Equipaje>
            {
                new Equipaje { Numero = "EQ001", IdPasajero = pasajeros[0].Id, Peso = 23.5m, Tipo = "Maleta" },
                new Equipaje { Numero = "EQ002", IdPasajero = pasajeros[1].Id, Peso = 18.0m, Tipo = "Maleta" },
                new Equipaje { Numero = "EQ003", IdPasajero = pasajeros[2].Id, Peso = 7.5m, Tipo = "Mochila" }
            };

            await context.Equipajes.AddRangeAsync(equipajes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEstadosVueloAsync(AppDbContext context)
        {
            // ⚠️ ESTADOS DE VUELO COMENTADOS - Dependen de vuelos que no existen
            Console.WriteLine("⚠️  Creación de estados de vuelo deshabilitada - Sin vuelos para asignar estados");
            return;

            /* ========== CÓDIGO COMENTADO ==========
         var vuelosHoy = await context.Vuelos
              .Where(v => v.Fecha == DateTime.Today)
            .Take(5)
                .ToListAsync();

            var estadosVuelo = new List<EstadoVuelo>();

               foreach (var vuelo in vuelosHoy)
              {
                estadosVuelo.Add(new EstadoVuelo
          {
              IdVuelo = vuelo.Id,
                Estado = "Embarcando",
               HoraSalidaProgramada = DateTime.Today.Add(vuelo.HoraSalida),
              HoraLlegadaProgramada = DateTime.Today.Add(vuelo.HoraLlegada),
               Puerta = $"A{new Random().Next(1, 20)}",
                  Observaciones = "Vuelo en tiempo"
          });
         }

                await context.EstadosVuelo.AddRangeAsync(estadosVuelo);
                await context.SaveChangesAsync();
               ========== FIN CÓDIGO COMENTADO ========== */
        }

        private static async Task SeedVueloTripulacionAsync(AppDbContext context)
        {
            // ⚠️ ASIGNACIÓN DE TRIPULACIÓN COMENTADA - Depende de vuelos que no existen
            Console.WriteLine("⚠️  Asignación de tripulación deshabilitada - Sin vuelos para asignar tripulantes");
            return;

            /* ========== CÓDIGO COMENTADO ==========
                 var vuelos = await context.Vuelos.Take(10).ToListAsync();
                   var pilotos = await context.Tripulaciones.Where(t => t.Rol == "Piloto").ToListAsync();
                    var copilotos = await context.Tripulaciones.Where(t => t.Rol == "Copiloto").ToListAsync();
                 var sobrecargos = await context.Tripulaciones.Where(t => t.Rol!.Contains("Sobrecargo")).ToListAsync();

                var vueloTripulaciones = new List<VueloTripulacion>();

            for (int i = 0; i < vuelos.Count; i++)
              {
                // Asignar piloto
                 vueloTripulaciones.Add(new VueloTripulacion
              {
                    IdVuelo = vuelos[i].Id,
            IdTripulacion = pilotos[i % pilotos.Count].Id
                });

                  // Asignar copiloto
            vueloTripulaciones.Add(new VueloTripulacion
                 {
            IdVuelo = vuelos[i].Id,
                   IdTripulacion = copilotos[i % copilotos.Count].Id
            });

                    // Asignar 4 sobrecargos
                for (int j = 0; j < 4; j++)
                  {
              vueloTripulaciones.Add(new VueloTripulacion
            {
             IdVuelo = vuelos[i].Id,
                   IdTripulacion = sobrecargos[(i * 4 + j) % sobrecargos.Count].Id
               });
              }
             }

                  await context.VueloTripulaciones.AddRangeAsync(vueloTripulacion);
               await context.SaveChangesAsync();
                  ========== FIN CÓDIGO COMENTADO ========== */
        }

        private static async Task SeedNotificacionesAsync(AppDbContext context)
        {
            var clientes = await context.Clientes.ToListAsync();

            if (!clientes.Any())
                return;

            var notificaciones = new List<Notificacion>
            {
                new Notificacion
                {
                    IdCliente = clientes[0].Id,
                    Tipo = "Confirmacion",
                    Mensaje = "Su reserva RES001 ha sido confirmada exitosamente.",
                    FechaEnvio = DateTime.Now.AddDays(-5),
                    Leida = true
                },
                new Notificacion
                {
                    IdCliente = clientes[0].Id,
                    Tipo = "Recordatorio",
                    Mensaje = "Recuerde hacer check-in 24 horas antes de su vuelo.",
                    FechaEnvio = DateTime.Now.AddDays(-1),
                    Leida = false
                }
            };

            await context.Notificaciones.AddRangeAsync(notificaciones);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza las fechas de los vuelos existentes para que siempre sean futuras
        /// </summary>
        private static async Task ActualizarFechasVuelosAsync(AppDbContext context)
        {
            var hoy = DateTime.Today;
 var vuelosEnPasado = await context.Vuelos
    .Where(v => v.Fecha < hoy)
      .ToListAsync();

    if (!vuelosEnPasado.Any())
            {
       Console.WriteLine("✅ No hay vuelos en el pasado para actualizar");
    return;
            }

 Console.WriteLine($"📅 Actualizando {vuelosEnPasado.Count} vuelos del pasado...");

            foreach (var vuelo in vuelosEnPasado)
 {
             // Calcular cuántos días en el pasado está el vuelo
       var diasEnPasado = (hoy - vuelo.Fecha).Days;

            // Mover el vuelo al futuro: mañana + el mismo offset relativo
    vuelo.Fecha = hoy.AddDays(1 + (diasEnPasado % 90));

  // Actualizar también la fecha de regreso si existe
    if (vuelo.FechaRegreso.HasValue && vuelo.FechaRegreso.Value < hoy)
  {
          var diasDespuesDeSalida = (vuelo.FechaRegreso.Value - vuelo.Fecha.AddDays(-diasEnPasado)).Days;
          vuelo.FechaRegreso = vuelo.Fecha.AddDays(Math.Max(1, diasDespuesDeSalida));
     }

                // ✅ NUEVO: Asegurar que ClasesDisponibles esté presente
      if (string.IsNullOrEmpty(vuelo.ClasesDisponibles))
      {
          vuelo.ClasesDisponibles = "Economica,Ejecutiva,Primera";
          Console.WriteLine($"   - Vuelo {vuelo.NumeroVuelo}: ClasesDisponibles actualizado");
         }
         }

 await context.SaveChangesAsync();
            Console.WriteLine($"✅ {vuelosEnPasado.Count} vuelos actualizados a fechas futuras");
            Console.WriteLine($"   - Primera fecha: {vuelosEnPasado.Min(v => v.Fecha):dd/MM/yyyy}");
      Console.WriteLine($"   - Última fecha: {vuelosEnPasado.Max(v => v.Fecha):dd/MM/yyyy}");
    }
    }
}