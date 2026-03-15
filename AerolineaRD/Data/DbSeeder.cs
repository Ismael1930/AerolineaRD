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

            // 14. RUTAS AÉREAS
            await SeedRutasAsync(context);

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

            // Cliente de prueba - Email consistente
            if (await userManager.FindByEmailAsync("ismaelfelizestudios@gmail.com") == null)
            {
                var clienteUser = new IdentityUser
                {
                    UserName = "ismaelfelizestudios@gmail.com",
                    Email = "ismaelfelizestudios@gmail.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(clienteUser, "Cliente123!");
                await userManager.AddToRoleAsync(clienteUser, "Cliente");
            }

            // Segundo cliente de prueba - Ramón Sánchez
            if (await userManager.FindByEmailAsync("ramonsanchez3177@gmail.com") == null)
            {
                var clienteUser2 = new IdentityUser
                {
                    UserName = "ramonsanchez3177@gmail.com",
                    Email = "ramonsanchez3177@gmail.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(clienteUser2, "Cliente123!");
                await userManager.AddToRoleAsync(clienteUser2, "Cliente");
            }
        }

        private static async Task SeedAeropuertosAsync(AppDbContext context)
        {
            var aeropuertos = new List<Aeropuerto>
    {
         // AEROPUERTO DE PRUEBA - Para demostrar validaciones
    new Aeropuerto { Codigo = "TEST", Nombre = "Aeropuerto de Prueba Validaciones", Ciudad = "Test City", Pais = "Republica Dominicana", CapacidadVuelosPorHora =1 },
            
       // Republica Dominicana
              new Aeropuerto { Codigo = "SDQ", Nombre = "Las Americas", Ciudad = "Santo Domingo", Pais = "Republica Dominicana", CapacidadVuelosPorHora =15 },
        new Aeropuerto { Codigo = "PUJ", Nombre = "Punta Cana", Ciudad = "Punta Cana", Pais = "Republica Dominicana", CapacidadVuelosPorHora =12 },
  new Aeropuerto { Codigo = "STI", Nombre = "Cibao", Ciudad = "Santiago", Pais = "Republica Dominicana", CapacidadVuelosPorHora =10 },
      new Aeropuerto { Codigo = "POP", Nombre = "Gregorio Luperon", Ciudad = "Puerto Plata", Pais = "Republica Dominicana", CapacidadVuelosPorHora =8 },
     new Aeropuerto { Codigo = "LRM", Nombre = "La Romana", Ciudad = "La Romana", Pais = "Republica Dominicana", CapacidadVuelosPorHora =6 },
          
     // Estados Unidos
 new Aeropuerto { Codigo = "JFK", Nombre = "John F. Kennedy", Ciudad = "Nueva York", Pais = "Estados Unidos", CapacidadVuelosPorHora =30 },
                new Aeropuerto { Codigo = "MIA", Nombre = "Miami International", Ciudad = "Miami", Pais = "Estados Unidos", CapacidadVuelosPorHora =25 },
    new Aeropuerto { Codigo = "EWR", Nombre = "Newark Liberty", Ciudad = "Newark", Pais = "Estados Unidos", CapacidadVuelosPorHora =28 },
  new Aeropuerto { Codigo = "FLL", Nombre = "Fort Lauderdale", Ciudad = "Fort Lauderdale", Pais = "Estados Unidos", CapacidadVuelosPorHora =20 },
     new Aeropuerto { Codigo = "ATL", Nombre = "Hartsfield-Jackson", Ciudad = "Atlanta", Pais = "Estados Unidos", CapacidadVuelosPorHora =35 },

                // Europa
      new Aeropuerto { Codigo = "MAD", Nombre = "Adolfo Suarez", Ciudad = "Madrid", Pais = "Espana", CapacidadVuelosPorHora =30 },
   new Aeropuerto { Codigo = "BCN", Nombre = "El Prat", Ciudad = "Barcelona", Pais = "Espana", CapacidadVuelosPorHora =25 },
     new Aeropuerto { Codigo = "CDG", Nombre = "Charles de Gaulle", Ciudad = "Paris", Pais = "Francia", CapacidadVuelosPorHora =32 },
        
        // Latinoamerica
 new Aeropuerto { Codigo = "CUN", Nombre = "Cancun", Ciudad = "Cancun", Pais = "Mexico", CapacidadVuelosPorHora =18 },
    new Aeropuerto { Codigo = "PTY", Nombre = "Tocumen", Ciudad = "Panama", Pais = "Panama", CapacidadVuelosPorHora =15 },
    new Aeropuerto { Codigo = "BOG", Nombre = "El Dorado", Ciudad = "Bogota", Pais = "Colombia", CapacidadVuelosPorHora =22 },
     new Aeropuerto { Codigo = "LIM", Nombre = "Jorge Chavez", Ciudad = "Lima", Pais = "Peru", CapacidadVuelosPorHora =20 }
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
    new Personal { Nombre = "Sobrecargo Andrés", Apellido = "Campos", Rol = "Sobrecargo", Licencia = "FA-S012", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-1), Activo = true },
    // ✅ NUEVO: Más sobrecargos para equipos Delta y Echo
    new Personal { Nombre = "Sobrecargo Carmen", Apellido = "Soto", Rol = "Sobrecargo", Licencia = "FA-S013", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
    new Personal { Nombre = "Sobrecargo Pablo", Apellido = "Díaz", Rol = "Sobrecargo", Licencia = "FA-S014", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-3), Activo = true },
    new Personal { Nombre = "Sobrecargo Elena", Apellido = "Torres", Rol = "Sobrecargo", Licencia = "FA-S015", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
    new Personal { Nombre = "Sobrecargo Ricardo", Apellido = "Flores", Rol = "Sobrecargo", Licencia = "FA-S016", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-1), Activo = true },
    new Personal { Nombre = "Sobrecargo Isabel", Apellido = "Ruiz", Rol = "Sobrecargo", Licencia = "FA-S017", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true },
    new Personal { Nombre = "Sobrecargo Diego", Apellido = "Vargas", Rol = "Sobrecargo", Licencia = "FA-S018", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-3), Activo = true },
    new Personal { Nombre = "Sobrecargo Marta", Apellido = "López", Rol = "Sobrecargo", Licencia = "FA-S019", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-1), Activo = true },
    new Personal { Nombre = "Sobrecargo Sergio", Apellido = "Martín", Rol = "Sobrecargo", Licencia = "FA-S020", TiempoDescansoMinutos = 480, Estado = "Disponible", FechaContratacion = DateTime.Today.AddYears(-2), Activo = true }
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
  },
    // ✅ NUEVO: Equipos adicionales para más aeronaves
    new Equipo
    {
        Nombre = "Equipo Delta",
        Codigo = "DELTA-01",
        Estado = "Disponible",
        FechaCreacion = DateTime.Today.AddMonths(-4),
        Activo = true
    },
    new Equipo
    {
        Nombre = "Equipo Echo",
        Codigo = "ECHO-01",
        Estado = "Disponible",
        FechaCreacion = DateTime.Today.AddMonths(-3),
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

            // ✅ EQUIPO DELTA (Completo y válido)
            var equipoDelta = equipos[3];
            equiposPersonal.AddRange(new[]
            {
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[2].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }, // Piloto 3
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[6].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }, // Copiloto 3
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[19].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }, // Sobrecargo 13
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[20].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }, // Sobrecargo 14
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[21].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }, // Sobrecargo 15
                new EquipoPersonal { IdEquipo = equipoDelta.Id, IdPersonal = personal[22].Id, FechaAsignacion = DateTime.Today.AddMonths(-4), Activo = true }  // Sobrecargo 16
            });

            // ✅ EQUIPO ECHO (Completo y válido)
            var equipoEcho = equipos[4];
            equiposPersonal.AddRange(new[]
            {
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[3].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }, // Piloto 4
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[7].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }, // Copiloto 4
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[23].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }, // Sobrecargo 17
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[24].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }, // Sobrecargo 18
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[25].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }, // Sobrecargo 19
                new EquipoPersonal { IdEquipo = equipoEcho.Id, IdPersonal = personal[26].Id, FechaAsignacion = DateTime.Today.AddMonths(-3), Activo = true }  // Sobrecargo 20
            });

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
},
    // ✅ NUEVO: Asignar Equipo Delta a HI-1003RD
    new AsignacionEquipoAeronave
    {
        IdEquipo = equipoDelta.Id,
        Matricula = "HI-1003RD",
        FechaAsignacion = DateTime.Today.AddMonths(-4),
        Activa = true,
        Observaciones = "Equipo Delta asignado a Airbus A320"
    },
    // ✅ NUEVO: Asignar Equipo Echo a HI-1004RD
    new AsignacionEquipoAeronave
    {
        IdEquipo = equipoEcho.Id,
        Matricula = "HI-1004RD",
        FechaAsignacion = DateTime.Today.AddMonths(-3),
        Activa = true,
        Observaciones = "Equipo Echo asignado a Airbus A320"
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
            // ✅ FECHAS FIJAS - Abril 2026 en adelante
            var fechaBase = new DateTime(2026, 4, 1); // 1 de abril 2026
            var dia1 = fechaBase;
            var dia2 = fechaBase.AddDays(1);
            var dia3 = fechaBase.AddDays(2);

            var vuelos = new List<Vuelo>();

            // ═══════════════════════════════════════════════════════════════════
            // ✈️ VUELOS DEL DÍA 1 (1 de Abril 2026)
            // ═══════════════════════════════════════════════════════════════════
            
            // Vuelo a las 11:00 AM
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD-ABR-1100",
                Fecha = dia1,
                HoraSalida = new TimeSpan(11, 0, 0),
                HoraLlegada = new TimeSpan(13, 30, 0),
                Duracion = 150,
                PrecioBase = 320.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "MIA",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // Vuelo a las 15:00 PM
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD-ABR-1500",
                Fecha = dia1,
                HoraSalida = new TimeSpan(15, 0, 0),
                HoraLlegada = new TimeSpan(19, 15, 0),
                Duracion = 255,
                PrecioBase = 450.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "JFK",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = dia1.AddDays(7),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // ═══════════════════════════════════════════════════════════════════
            // ✈️ VUELOS DEL DÍA 2 (2 de Abril 2026)
            // ═══════════════════════════════════════════════════════════════════
            
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD1001",
                Fecha = dia2,
                HoraSalida = new TimeSpan(6, 0, 0),
                HoraLlegada = new TimeSpan(8, 30, 0),
                Duracion = 150,
                PrecioBase = 320.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "MIA",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD1002",
                Fecha = dia2,
                HoraSalida = new TimeSpan(11, 0, 0),
                HoraLlegada = new TimeSpan(15, 15, 0),
                Duracion = 255,
                PrecioBase = 450.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "JFK",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = dia2.AddDays(7),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD1003",
                Fecha = dia2,
                HoraSalida = new TimeSpan(14, 0, 0),
                HoraLlegada = new TimeSpan(16, 45, 0),
                Duracion = 165,
                PrecioBase = 350.00m,
                OrigenCodigo = "PUJ",
                DestinoCodigo = "MIA",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // ✈️ VUELO NOCTURNO - Cruza medianoche
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD1004-NOCHE",
                Fecha = dia2,
                HoraSalida = new TimeSpan(22, 15, 0),
                HoraLlegada = new TimeSpan(0, 45, 0),
                Duracion = 150,
                PrecioBase = 380.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "MIA",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // ═══════════════════════════════════════════════════════════════════
            // ✈️ VUELOS DEL DÍA 3 (3 de Abril 2026)
            // ═══════════════════════════════════════════════════════════════════
            
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD2001",
                Fecha = dia3,
                HoraSalida = new TimeSpan(7, 30, 0),
                HoraLlegada = new TimeSpan(8, 0, 0),
                Duracion = 30,
                PrecioBase = 150.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "PUJ",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD2002",
                Fecha = dia3,
                HoraSalida = new TimeSpan(10, 0, 0),
                HoraLlegada = new TimeSpan(19, 0, 0),
                Duracion = 540,
                PrecioBase = 850.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "MAD",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = dia3.AddDays(14),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD2003",
                Fecha = dia3,
                HoraSalida = new TimeSpan(9, 0, 0),
                HoraLlegada = new TimeSpan(13, 30, 0),
                Duracion = 270,
                PrecioBase = 480.00m,
                OrigenCodigo = "PUJ",
                DestinoCodigo = "JFK",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // ═══════════════════════════════════════════════════════════════════
            // ✈️ VUELOS DÍAS 4-8 (4-8 de Abril 2026)
            // ═══════════════════════════════════════════════════════════════════
            
            // Día 4 (4 de Abril)
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD3001",
                Fecha = fechaBase.AddDays(3),
                HoraSalida = new TimeSpan(6, 30, 0),
                HoraLlegada = new TimeSpan(9, 50, 0),
                Duracion = 200,
                PrecioBase = 400.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "ATL",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD3002",
                Fecha = fechaBase.AddDays(3),
                HoraSalida = new TimeSpan(8, 0, 0),
                HoraLlegada = new TimeSpan(11, 15, 0),
                Duracion = 195,
                PrecioBase = 420.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "PTY",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = fechaBase.AddDays(8),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // Día 5 (5 de Abril) - Vuelos a Colombia y México
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD4001",
                Fecha = fechaBase.AddDays(4),
                HoraSalida = new TimeSpan(7, 0, 0),
                HoraLlegada = new TimeSpan(10, 30, 0),
                Duracion = 210,
                PrecioBase = 380.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "BOG",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD4002",
                Fecha = fechaBase.AddDays(4),
                HoraSalida = new TimeSpan(12, 0, 0),
                HoraLlegada = new TimeSpan(15, 0, 0),
                Duracion = 180,
                PrecioBase = 420.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "CUN",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = fechaBase.AddDays(14),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // Día 6 (6 de Abril) - Vuelos nacionales
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD5001",
                Fecha = fechaBase.AddDays(5),
                HoraSalida = new TimeSpan(8, 0, 0),
                HoraLlegada = new TimeSpan(8, 35, 0),
                Duracion = 35,
                PrecioBase = 120.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "STI",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD5002",
                Fecha = fechaBase.AddDays(5),
                HoraSalida = new TimeSpan(10, 0, 0),
                HoraLlegada = new TimeSpan(10, 40, 0),
                Duracion = 40,
                PrecioBase = 130.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "POP",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // Día 7 (7 de Abril) - Vuelo largo a Europa (nocturno)
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD6001",
                Fecha = fechaBase.AddDays(6),
                HoraSalida = new TimeSpan(21, 0, 0),
                HoraLlegada = new TimeSpan(6, 30, 0),
                Duracion = 570,
                PrecioBase = 920.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "BCN",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = fechaBase.AddDays(18),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // Día 8 (8 de Abril) - Vuelos a Perú y Francia
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD7001",
                Fecha = fechaBase.AddDays(7),
                HoraSalida = new TimeSpan(6, 0, 0),
                HoraLlegada = new TimeSpan(11, 30, 0),
                Duracion = 330,
                PrecioBase = 650.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "LIM",
                Matricula = "HI-1001RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "RD7002",
                Fecha = fechaBase.AddDays(7),
                HoraSalida = new TimeSpan(20, 0, 0),
                HoraLlegada = new TimeSpan(5, 15, 0),
                Duracion = 555,
                PrecioBase = 950.00m,
                OrigenCodigo = "SDQ",
                DestinoCodigo = "CDG",
                Matricula = "HI-1002RD",
                Estado = "Programado",
                TipoVuelo = "IdaYVuelta",
                FechaRegreso = fechaBase.AddDays(21),
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // ═══════════════════════════════════════════════════════════════════
            // 🧪 VUELOS PARA PROBAR VALIDACIÓN DE CAPACIDAD DE AEROPUERTO TEST
            // Aeropuerto TEST tiene capacidad de 1 vuelo por hora
            // ═══════════════════════════════════════════════════════════════════
            
            // VUELOS DEL DÍA 1 en TEST (1 de Abril)
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-ABR-1100",
                Fecha = dia1,
                HoraSalida = new TimeSpan(11, 0, 0),
                HoraLlegada = new TimeSpan(11, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "SDQ",
                Matricula = "HI-1003RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-ABR-1200",
                Fecha = dia1,
                HoraSalida = new TimeSpan(12, 0, 0),
                HoraLlegada = new TimeSpan(12, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "PUJ",
                Matricula = "HI-1004RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-ABR-1400",
                Fecha = dia1,
                HoraSalida = new TimeSpan(14, 0, 0),
                HoraLlegada = new TimeSpan(14, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "SDQ",
                Matricula = "HI-1003RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-ABR-1500",
                Fecha = dia1,
                HoraSalida = new TimeSpan(15, 0, 0),
                HoraLlegada = new TimeSpan(15, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "PUJ",
                Matricula = "HI-1004RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            // VUELOS DEL DÍA 2 en TEST (2 de Abril)
            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-1000",
                Fecha = dia2,
                HoraSalida = new TimeSpan(10, 0, 0),
                HoraLlegada = new TimeSpan(10, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "SDQ",
                Matricula = "HI-1003RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-1100",
                Fecha = dia2,
                HoraSalida = new TimeSpan(11, 0, 0),
                HoraLlegada = new TimeSpan(11, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "PUJ",
                Matricula = "HI-1004RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-1200",
                Fecha = dia2,
                HoraSalida = new TimeSpan(12, 0, 0),
                HoraLlegada = new TimeSpan(12, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "SDQ",
                Matricula = "HI-1003RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-1400",
                Fecha = dia2,
                HoraSalida = new TimeSpan(14, 0, 0),
                HoraLlegada = new TimeSpan(14, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "PUJ",
                Matricula = "HI-1003RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            vuelos.Add(new Vuelo
            {
                NumeroVuelo = "TEST-1500",
                Fecha = dia2,
                HoraSalida = new TimeSpan(15, 0, 0),
                HoraLlegada = new TimeSpan(15, 30, 0),
                Duracion = 30,
                PrecioBase = 100.00m,
                OrigenCodigo = "TEST",
                DestinoCodigo = "SDQ",
                Matricula = "HI-1004RD",
                Estado = "Programado",
                TipoVuelo = "SoloIda",
                ClasesDisponibles = "Economica,Ejecutiva,Primera"
            });

            await context.Vuelos.AddRangeAsync(vuelos);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ {vuelos.Count} vuelos creados:");
            Console.WriteLine($"      - Fecha base: {fechaBase:dd/MM/yyyy} (Abril 2026)");
            Console.WriteLine($"      - 2 vuelos del 1 de Abril (11:00 y 15:00)");
            Console.WriteLine($"      - 4 vuelos del 1 de Abril en aeropuerto TEST");
            Console.WriteLine($"      - 5 vuelos del 2 de Abril en aeropuerto TEST");
            Console.WriteLine($"      - Horas LIBRES en TEST: 13:00, 16:00+");
            Console.WriteLine($"      - {vuelos.Count(v => v.HoraLlegada < v.HoraSalida)} vuelos nocturnos (cruzan medianoche)");
            Console.WriteLine($"      - Aeronaves utilizadas: HI-1001RD, HI-1002RD, HI-1003RD, HI-1004RD");
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
            // Si ya existen clientes, no duplicar
            if (await context.Clientes.AnyAsync())
            {
                Console.WriteLine("⏭️  Clientes ya existen, saltando seed");
                return;
            }

            var clientes = new List<Cliente>();

            // Cliente 1: Ismael (vinculado a usuario)
            var ismaelUser = await userManager.FindByEmailAsync("ismaelfelizestudios@gmail.com");
            if (ismaelUser != null)
            {
                clientes.Add(new Cliente
                {
                    Nombre = "Ismael Féliz",
                    Email = "ismaelfelizestudios@gmail.com",
                    Telefono = "+1809-555-0001",
                    UserId = ismaelUser.Id
                });
            }

            // Cliente 2: Ramón Sánchez (vinculado a usuario)
            var ramonUser = await userManager.FindByEmailAsync("ramonsanchez3177@gmail.com");
            if (ramonUser != null)
            {
                clientes.Add(new Cliente
                {
                    Nombre = "Ramón Sánchez",
                    Email = "ramonsanchez3177@gmail.com",
                    Telefono = "+1809-555-0002",
                    UserId = ramonUser.Id
                });
            }

            // Clientes adicionales sin usuario (para pruebas)
            clientes.AddRange(new[]
            {
                new Cliente { Nombre = "María González", Email = "maria.gonzalez@example.com", Telefono = "+1809-555-0003" },
                new Cliente { Nombre = "Pedro Martínez", Email = "pedro.martinez@example.com", Telefono = "+1809-555-0004" }
            });

            await context.Clientes.AddRangeAsync(clientes);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ {clientes.Count} clientes creados");
            foreach (var c in clientes)
            {
                Console.WriteLine($"      - {c.Nombre} ({c.Email}) - UserId: {c.UserId ?? "N/A"}");
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
            {
                Console.WriteLine("⏭️  Pasajeros ya existen, saltando seed");
                return;
            }

            // Obtener clientes existentes para vincular pasajeros
            var clientes = await context.Clientes.ToListAsync();

            if (!clientes.Any())
            {
                Console.WriteLine("⚠️  No hay clientes para vincular pasajeros");
                return;
            }

            var pasajeros = new List<Pasajero>();

            // Crear un pasajero para cada cliente (vinculado)
            foreach (var cliente in clientes)
            {
                string pasaporte;
                do
                {
                    pasaporte = GeneratePasaporte(rnd);
                } while (pasajeros.Any(p => p.Pasaporte == pasaporte));

                // Extraer nombre y apellido del cliente
                var nombreCompleto = cliente.Nombre ?? "Cliente Desconocido";
                var partes = nombreCompleto.Split(' ', 2);
                var nombre = partes[0];
                var apellido = partes.Length > 1 ? partes[1] : "Sin Apellido";

                var pasajero = new Pasajero
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Pasaporte = pasaporte,
                    IdCliente = cliente.Id // ✅ Vinculado al cliente
                };
                pasajeros.Add(pasajero);

                Console.WriteLine($"   👤 Pasajero: {nombre} {apellido} → Cliente ID: {cliente.Id} ({cliente.Email})");
            }

            await context.Pasajeros.AddRangeAsync(pasajeros);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ {pasajeros.Count} pasajeros creados y vinculados a clientes");
        }

        private static async Task SeedReservasAsync(AppDbContext context)
        {
            // ⚠️ RESERVAS DESHABILITADAS - El usuario las creará manualmente
            Console.WriteLine("⏭️  Seed de reservas deshabilitado - Crear reservas manualmente via API");
            return;
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

        /// <summary>
        /// Seed de rutas aéreas con duraciones estimadas entre aeropuertos
        /// </summary>
        private static async Task SeedRutasAsync(AppDbContext context)
        {
            // Si ya existen rutas, no duplicar
            if (await context.Rutas.AnyAsync())
            {
                Console.WriteLine("⏭️  Rutas ya existen, saltando seed");
                return;
            }

            var rutas = new List<Ruta>();

            // ========== RUTAS DESDE AEROPUERTO TEST (Para validaciones) ==========
            // Rutas desde TEST hacia TODOS los aeropuertos
            rutas.AddRange(new[]
            {
                // Nacionales RD
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "SDQ", DuracionMinutos = 30, DistanciaKm = 50 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "PUJ", DuracionMinutos = 35, DistanciaKm = 60 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "STI", DuracionMinutos = 25, DistanciaKm = 45 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "POP", DuracionMinutos = 30, DistanciaKm = 55 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "LRM", DuracionMinutos = 20, DistanciaKm = 40 },
                
                // Estados Unidos
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "JFK", DuracionMinutos = 255, DistanciaKm = 2540 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "MIA", DuracionMinutos = 150, DistanciaKm = 1350 },
                new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "EWR", DuracionMinutos = 260, DistanciaKm = 2500 },
  new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "FLL", DuracionMinutos = 155, DistanciaKm = 1350 },
     new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "ATL", DuracionMinutos = 200, DistanciaKm = 2050 },

                // Europa
      new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "MAD", DuracionMinutos = 540, DistanciaKm = 6900 },
   new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "BCN", DuracionMinutos = 570, DistanciaKm = 7100 },
     new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "CDG", DuracionMinutos = 555, DistanciaKm = 7050 },
        
        // Latinoamérica
 new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "CUN", DuracionMinutos = 180, DistanciaKm = 1750 },
    new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "PTY", DuracionMinutos = 195, DistanciaKm = 1900 },
    new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "BOG", DuracionMinutos = 210, DistanciaKm = 1950 },
     new Ruta { OrigenCodigo = "TEST", DestinoCodigo = "LIM", DuracionMinutos = 720, DistanciaKm = 9900 },
            });

            // Rutas HACIA TEST desde todos los aeropuertos (para vuelos de regreso)
            rutas.AddRange(new[]
            {
                // Nacionales RD
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "TEST", DuracionMinutos = 30, DistanciaKm = 50 },
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "TEST", DuracionMinutos = 35, DistanciaKm = 60 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "TEST", DuracionMinutos = 25, DistanciaKm = 45 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "TEST", DuracionMinutos = 30, DistanciaKm = 55 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "TEST", DuracionMinutos = 20, DistanciaKm = 40 },
                
                // Estados Unidos
                new Ruta { OrigenCodigo = "JFK", DestinoCodigo = "TEST", DuracionMinutos = 255, DistanciaKm = 2540 },
                new Ruta { OrigenCodigo = "MIA", DestinoCodigo = "TEST", DuracionMinutos = 150, DistanciaKm = 1350 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "TEST", DuracionMinutos = 260, DistanciaKm = 2500 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "TEST", DuracionMinutos = 155, DistanciaKm = 1350 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "TEST", DuracionMinutos = 200, DistanciaKm = 2050 },
                
                // Europa
                new Ruta { OrigenCodigo = "MAD", DestinoCodigo = "TEST", DuracionMinutos = 540, DistanciaKm = 6900 },
                new Ruta { OrigenCodigo = "BCN", DestinoCodigo = "TEST", DuracionMinutos = 570, DistanciaKm = 7100 },
                new Ruta { OrigenCodigo = "CDG", DestinoCodigo = "TEST", DuracionMinutos = 555, DistanciaKm = 7050 },
                
                // Latinoamérica
                new Ruta { OrigenCodigo = "CUN", DestinoCodigo = "TEST", DuracionMinutos = 180, DistanciaKm = 1750 },
                new Ruta { OrigenCodigo = "PTY", DestinoCodigo = "TEST", DuracionMinutos = 195, DistanciaKm = 1900 },
                new Ruta { OrigenCodigo = "BOG", DestinoCodigo = "TEST", DuracionMinutos = 210, DistanciaKm = 1950 },
                new Ruta { OrigenCodigo = "LIM", DestinoCodigo = "TEST", DuracionMinutos = 720, DistanciaKm = 9900 },
            });

            // ========== RUTAS DESDE REPÚBLICA DOMINICANA ==========
            
            // Desde Santo Domingo (SDQ)
            rutas.AddRange(new[]
            {
                // Nacionales
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "PUJ", DuracionMinutos = 30, DistanciaKm = 150 },
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "STI", DuracionMinutos = 35, DistanciaKm = 155 },
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "POP", DuracionMinutos = 40, DistanciaKm = 215 },
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "LRM", DuracionMinutos = 25, DistanciaKm = 110 },
                
                // Estados Unidos
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "JFK", DuracionMinutos = 255, DistanciaKm = 2540 },  // 4h 15m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "MIA", DuracionMinutos = 150, DistanciaKm = 1330 },  // 2h 30m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "EWR", DuracionMinutos = 260, DistanciaKm = 2500 },  // 4h 20m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "FLL", DuracionMinutos = 155, DistanciaKm = 1350 },  // 2h 35m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "ATL", DuracionMinutos = 200, DistanciaKm = 2050 },  // 3h 20m
                
                // Europa
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "MAD", DuracionMinutos = 540, DistanciaKm = 6900 },  // 9h
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "BCN", DuracionMinutos = 570, DistanciaKm = 7100 },  // 9h 30m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "CDG", DuracionMinutos = 555, DistanciaKm = 7050 },  // 9h 15m
                
                // Latinoamérica
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "CUN", DuracionMinutos = 180, DistanciaKm = 1750 },  // 3h
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "PTY", DuracionMinutos = 195, DistanciaKm = 1900 },  // 3h 15m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "BOG", DuracionMinutos = 210, DistanciaKm = 1950 },  // 3h 30m
                new Ruta { OrigenCodigo = "SDQ", DestinoCodigo = "LIM", DuracionMinutos = 330, DistanciaKm = 3600 },  // 5h 30m
            });

            // Desde Punta Cana (PUJ)
            rutas.AddRange(new[]
            {
                // Nacionales
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "SDQ", DuracionMinutos = 30, DistanciaKm = 150 },
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "STI", DuracionMinutos = 45, DistanciaKm = 200 },
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "POP", DuracionMinutos = 50, DistanciaKm = 230 },  // NUEVO
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "LRM", DuracionMinutos = 20, DistanciaKm = 70 },
                
                // Estados Unidos
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "JFK", DuracionMinutos = 270, DistanciaKm = 2600 },  // 4h 30m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "MIA", DuracionMinutos = 165, DistanciaKm = 1400 },  // 2h 45m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "EWR", DuracionMinutos = 275, DistanciaKm = 2550 },
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "FLL", DuracionMinutos = 170, DistanciaKm = 1420 },  // 2h 50m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "ATL", DuracionMinutos = 185, DistanciaKm = 2100 },  // 3h 05m
                
                // Europa
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "MAD", DuracionMinutos = 555, DistanciaKm = 6950 },  // 9h 15m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "BCN", DuracionMinutos = 585, DistanciaKm = 7150 },  // 9h 45m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "CDG", DuracionMinutos = 570, DistanciaKm = 7100 },  // 9h 30m
                
                // Latinoamérica
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "CUN", DuracionMinutos = 195, DistanciaKm = 1800 },  // 3h 15m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "PTY", DuracionMinutos = 210, DistanciaKm = 1950 },  // 3h 30m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "BOG", DuracionMinutos = 225, DistanciaKm = 2000 },  // 3h 45m
                new Ruta { OrigenCodigo = "PUJ", DestinoCodigo = "LIM", DuracionMinutos = 345, DistanciaKm = 3650 },  // 5h 45m
            });

            // Desde Santiago (STI)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "SDQ", DuracionMinutos = 35, DistanciaKm = 155 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "PUJ", DuracionMinutos = 45, DistanciaKm = 200 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "POP", DuracionMinutos = 25, DistanciaKm = 100 },  // NUEVO
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "LRM", DuracionMinutos = 40, DistanciaKm = 180 },  // NUEVO
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "MIA", DuracionMinutos = 150, DistanciaKm = 1320 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "JFK", DuracionMinutos = 250, DistanciaKm = 2480 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "EWR", DuracionMinutos = 255, DistanciaKm = 2450 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "FLL", DuracionMinutos = 155, DistanciaKm = 1340 },
                new Ruta { OrigenCodigo = "STI", DestinoCodigo = "ATL", DuracionMinutos = 195, DistanciaKm = 2000 },
            });

            // Desde Puerto Plata (POP)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "SDQ", DuracionMinutos = 40, DistanciaKm = 215 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "PUJ", DuracionMinutos = 50, DistanciaKm = 230 },  // NUEVO
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "STI", DuracionMinutos = 25, DistanciaKm = 100 },  // NUEVO
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "LRM", DuracionMinutos = 45, DistanciaKm = 200 },  // NUEVO
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "JFK", DuracionMinutos = 240, DistanciaKm = 2400 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "MIA", DuracionMinutos = 155, DistanciaKm = 1300 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "EWR", DuracionMinutos = 245, DistanciaKm = 2380 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "FLL", DuracionMinutos = 160, DistanciaKm = 1320 },
                new Ruta { OrigenCodigo = "POP", DestinoCodigo = "ATL", DuracionMinutos = 190, DistanciaKm = 1980 },
            });

            // Desde La Romana (LRM)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "SDQ", DuracionMinutos = 25, DistanciaKm = 110 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "PUJ", DuracionMinutos = 20, DistanciaKm = 70 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "STI", DuracionMinutos = 40, DistanciaKm = 180 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "POP", DuracionMinutos = 45, DistanciaKm = 200 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "MIA", DuracionMinutos = 160, DistanciaKm = 1360 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "JFK", DuracionMinutos = 265, DistanciaKm = 2560 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "EWR", DuracionMinutos = 270, DistanciaKm = 2520 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "FLL", DuracionMinutos = 165, DistanciaKm = 1380 },
                new Ruta { OrigenCodigo = "LRM", DestinoCodigo = "ATL", DuracionMinutos = 205, DistanciaKm = 2080 },
            });

            // ========== RUTAS ADICIONALES DESDE EE.UU. ==========
            
            // Desde Newark (EWR)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "SDQ", DuracionMinutos = 260, DistanciaKm = 2500 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "PUJ", DuracionMinutos = 275, DistanciaKm = 2550 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "STI", DuracionMinutos = 255, DistanciaKm = 2450 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "POP", DuracionMinutos = 245, DistanciaKm = 2380 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "LRM", DuracionMinutos = 270, DistanciaKm = 2520 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "MIA", DuracionMinutos = 185, DistanciaKm = 1760 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "JFK", DuracionMinutos = 30, DistanciaKm = 35 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "FLL", DuracionMinutos = 190, DistanciaKm = 1780 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "ATL", DuracionMinutos = 140, DistanciaKm = 1210 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "MAD", DuracionMinutos = 455, DistanciaKm = 5780 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "CDG", DuracionMinutos = 440, DistanciaKm = 5850 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "BCN", DuracionMinutos = 470, DistanciaKm = 6100 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "CUN", DuracionMinutos = 230, DistanciaKm = 2120 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "PTY", DuracionMinutos = 335, DistanciaKm = 3570 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "BOG", DuracionMinutos = 365, DistanciaKm = 3920 },
                new Ruta { OrigenCodigo = "EWR", DestinoCodigo = "LIM", DuracionMinutos = 480, DistanciaKm = 5400 },
            });

            // Desde Fort Lauderdale (FLL)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "SDQ", DuracionMinutos = 155, DistanciaKm = 1350 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "PUJ", DuracionMinutos = 170, DistanciaKm = 1420 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "STI", DuracionMinutos = 155, DistanciaKm = 1340 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "POP", DuracionMinutos = 160, DistanciaKm = 1320 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "LRM", DuracionMinutos = 165, DistanciaKm = 1380 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "MIA", DuracionMinutos = 30, DistanciaKm = 45 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "JFK", DuracionMinutos = 175, DistanciaKm = 1700 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "EWR", DuracionMinutos = 190, DistanciaKm = 1780 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "ATL", DuracionMinutos = 110, DistanciaKm = 1000 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "CUN", DuracionMinutos = 95, DistanciaKm = 870 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "PTY", DuracionMinutos = 185, DistanciaKm = 2070 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "BOG", DuracionMinutos = 230, DistanciaKm = 2620 },
                new Ruta { OrigenCodigo = "FLL", DestinoCodigo = "LIM", DuracionMinutos = 335, DistanciaKm = 4200 },
            });

            // Completar rutas desde Atlanta (ATL)
            rutas.AddRange(new[]
            {
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "SDQ", DuracionMinutos = 210, DistanciaKm = 2050 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "PUJ", DuracionMinutos = 220, DistanciaKm = 2150 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "STI", DuracionMinutos = 195, DistanciaKm = 2000 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "POP", DuracionMinutos = 190, DistanciaKm = 1980 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "LRM", DuracionMinutos = 205, DistanciaKm = 2080 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "EWR", DuracionMinutos = 140, DistanciaKm = 1210 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "FLL", DuracionMinutos = 110, DistanciaKm = 1000 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "PTY", DuracionMinutos = 240, DistanciaKm = 2650 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "BOG", DuracionMinutos = 270, DistanciaKm = 3100 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "LIM", DuracionMinutos = 390, DistanciaKm = 4600 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "MAD", DuracionMinutos = 510, DistanciaKm = 6800 },
                new Ruta { OrigenCodigo = "ATL", DestinoCodigo = "CDG", DuracionMinutos = 525, DistanciaKm = 7000 },
            });

            // Marcar todas las rutas como activas
            foreach (var ruta in rutas)
            {
                ruta.Activa = true;
            }

            await context.Rutas.AddRangeAsync(rutas);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ {rutas.Count} rutas aéreas creadas");
            Console.WriteLine($"      - Rutas nacionales RD: {rutas.Count(r => new[] { "SDQ", "PUJ", "STI", "POP", "LRM" }.Contains(r.OrigenCodigo) && new[] { "SDQ", "PUJ", "STI", "POP", "LRM" }.Contains(r.DestinoCodigo))}");
            Console.WriteLine($"      - Rutas internacionales: {rutas.Count(r => !(new[] { "SDQ", "PUJ", "STI", "POP", "LRM" }.Contains(r.OrigenCodigo) && new[] { "SDQ", "PUJ", "STI", "POP", "LRM" }.Contains(r.DestinoCodigo)))}");
        }
    }
}