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
            Console.WriteLine($"   - {await context.Reservas.CountAsync()} reservas");
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
                // República Dominicana
                new Aeropuerto { Codigo = "SDQ", Nombre = "Las Américas", Ciudad = "Santo Domingo", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "PUJ", Nombre = "Punta Cana", Ciudad = "Punta Cana", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "STI", Nombre = "Cibao", Ciudad = "Santiago", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "POP", Nombre = "Gregorio Luperón", Ciudad = "Puerto Plata", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "LRM", Nombre = "La Romana", Ciudad = "La Romana", Pais = "República Dominicana" },
                
                // Estados Unidos
                new Aeropuerto { Codigo = "JFK", Nombre = "John F. Kennedy", Ciudad = "Nueva York", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "MIA", Nombre = "Miami International", Ciudad = "Miami", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "EWR", Nombre = "Newark Liberty", Ciudad = "Newark", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "FLL", Nombre = "Fort Lauderdale", Ciudad = "Fort Lauderdale", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "ATL", Nombre = "Hartsfield-Jackson", Ciudad = "Atlanta", Pais = "Estados Unidos" },
                
                // Europa
                new Aeropuerto { Codigo = "MAD", Nombre = "Adolfo Suárez", Ciudad = "Madrid", Pais = "España" },
                new Aeropuerto { Codigo = "BCN", Nombre = "El Prat", Ciudad = "Barcelona", Pais = "España" },
                new Aeropuerto { Codigo = "CDG", Nombre = "Charles de Gaulle", Ciudad = "París", Pais = "Francia" },
        
                // Latinoamérica
                new Aeropuerto { Codigo = "CUN", Nombre = "Cancún", Ciudad = "Cancún", Pais = "México" },
                new Aeropuerto { Codigo = "PTY", Nombre = "Tocumen", Ciudad = "Panamá", Pais = "Panamá" },
                new Aeropuerto { Codigo = "BOG", Nombre = "El Dorado", Ciudad = "Bogotá", Pais = "Colombia" },
                new Aeropuerto { Codigo = "LIM", Nombre = "Jorge Chávez", Ciudad = "Lima", Pais = "Perú" }
            };

            await context.Aeropuertos.AddRangeAsync(aeropuertos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAeronavesAsync(AppDbContext context)
        {
            var aeronaves = new List<Aeronave>
            {
                new Aeronave { Matricula = "HI-1001RD", Modelo = "Boeing 737-800", Capacidad = 189, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1002RD", Modelo = "Boeing 737-800", Capacidad = 189, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1003RD", Modelo = "Airbus A320", Capacidad = 180, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1004RD", Modelo = "Airbus A320", Capacidad = 180, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1005RD", Modelo = "Boeing 787-9", Capacidad = 296, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1006RD", Modelo = "Airbus A321", Capacidad = 220, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1007RD", Modelo = "Boeing 737-MAX", Capacidad = 178, Estado = "Operativa" },
                new Aeronave { Matricula = "HI-1008RD", Modelo = "Embraer E195", Capacidad = 132, Estado = "Mantenimiento" }
            };

            await context.Aeronaves.AddRangeAsync(aeronaves);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTripulacionAsync(AppDbContext context)
        {
            var tripulacion = new List<Tripulacion>
            {
                // Pilotos
                new Tripulacion { Nombre = "Carlos", Apellido = "Rodríguez", Rol = "Piloto", Licencia = "ATP-001" },
                new Tripulacion { Nombre = "María", Apellido = "Santos", Rol = "Piloto", Licencia = "ATP-002" },
                new Tripulacion { Nombre = "Juan", Apellido = "Pérez", Rol = "Piloto", Licencia = "ATP-003" },
                new Tripulacion { Nombre = "Ana", Apellido = "Martínez", Rol = "Piloto", Licencia = "ATP-004" },
            
                // Copilotos
                new Tripulacion { Nombre = "Luis", Apellido = "García", Rol = "Copiloto", Licencia = "CPL-001" },
                new Tripulacion { Nombre = "Carmen", Apellido = "López", Rol = "Copiloto", Licencia = "CPL-002" },
                new Tripulacion { Nombre = "Pedro", Apellido = "Hernández", Rol = "Copiloto", Licencia = "CPL-003" },
                new Tripulacion { Nombre = "Isabel", Apellido = "Gómez", Rol = "Copiloto", Licencia = "CPL-004" },
            
                // Sobrecargos
                new Tripulacion { Nombre = "Rosa", Apellido = "Díaz", Rol = "Sobrecargo Jefe", Licencia = "FA-001" },
                new Tripulacion { Nombre = "Miguel", Apellido = "Torres", Rol = "Sobrecargo", Licencia = "FA-002" },
                new Tripulacion { Nombre = "Laura", Apellido = "Ramírez", Rol = "Sobrecargo", Licencia = "FA-003" },
                new Tripulacion { Nombre = "José", Apellido = "Flores", Rol = "Sobrecargo", Licencia = "FA-004" },
                new Tripulacion { Nombre = "Patricia", Apellido = "Morales", Rol = "Sobrecargo", Licencia = "FA-005" },
                new Tripulacion { Nombre = "Roberto", Apellido = "Cruz", Rol = "Sobrecargo", Licencia = "FA-006" }
            };

            await context.Tripulaciones.AddRangeAsync(tripulacion);
            await context.SaveChangesAsync();
        }

        private static async Task SeedVuelosAsync(AppDbContext context)
        {
            var hoy = DateTime.Today;
            var vuelos = new List<Vuelo>();
            var matriculas = new[] { "HI-1001RD", "HI-1002RD", "HI-1003RD", "HI-1004RD", "HI-1005RD", "HI-1006RD", "HI-1007RD" };
            var random = new Random();

            // Rutas de ejemplo para generar vuelos
            var rutasBase = new[]
                   {
    (origen: "SDQ", destino: "JFK", precio: 450.00m, duracion: 255),
         (origen: "SDQ", destino: "MIA", precio: 320.00m, duracion: 150),
    (origen: "SDQ", destino: "ATL", precio: 380.00m, duracion: 180),
 (origen: "SDQ", destino: "MAD", precio: 850.00m, duracion: 540),
     (origen: "SDQ", destino: "CDG", precio: 920.00m, duracion: 600),
     (origen: "PUJ", destino: "JFK", precio: 480.00m, duracion: 270),
  (origen: "PUJ", destino: "MIA", precio: 340.00m, duracion: 160),
   (origen: "PUJ", destino: "EWR", precio: 460.00m, duracion: 265),
       (origen: "PUJ", destino: "CDG", precio: 920.00m, duracion: 600),
         (origen: "STI", destino: "MIA", precio: 340.00m, duracion: 150),
        (origen: "STI", destino: "JFK", precio: 470.00m, duracion: 260),
   (origen: "POP", destino: "MIA", precio: 330.00m, duracion: 145),
   (origen: "POP", destino: "CUN", precio: 280.00m, duracion: 120),
  (origen: "SDQ", destino: "CUN", precio: 380.00m, duracion: 150),
     (origen: "SDQ", destino: "PTY", precio: 320.00m, duracion: 130),
       (origen: "SDQ", destino: "BOG", precio: 420.00m, duracion: 180),
  (origen: "SDQ", destino: "LIM", precio: 550.00m, duracion: 300),
           (origen: "PUJ", destino: "BCN", precio: 980.00m, duracion: 620),
          (origen: "STI", destino: "ATL", precio: 390.00m, duracion: 185),
          (origen: "LRM", destino: "MIA", precio: 340.00m, duracion: 155)
       };

            int numeroVuelo = 1000;

            // Generar 100 vuelos de IDA Y VUELTA
            for (int i = 0; i < 100; i++)
            {
                var ruta = rutasBase[i % rutasBase.Length];
                var dia = i / rutasBase.Length;
                var horaSalida = new TimeSpan(6 + (i % 16), random.Next(0, 60), 0); // Entre 6 AM y 10 PM
                var horaLlegada = horaSalida.Add(TimeSpan.FromMinutes(ruta.duracion));
                var fechaSalida = hoy.AddDays(dia);
                var fechaRegreso = fechaSalida.AddDays(7 + random.Next(0, 14)); // Entre 7 y 21 días después

                vuelos.Add(new Vuelo
                {
                    NumeroVuelo = $"RD{numeroVuelo++}",
                    Fecha = fechaSalida,
                    HoraSalida = horaSalida,
                    HoraLlegada = horaLlegada,
                    Duracion = ruta.duracion,
                    PrecioBase = ruta.precio,
                    OrigenCodigo = ruta.origen,
                    DestinoCodigo = ruta.destino,
                    Matricula = matriculas[i % matriculas.Length],
                    Estado = "Programado",
                    TipoVuelo = "IdaYVuelta",
                    //FechaRegreso = fechaRegreso
                });
            }

            // Generar 100 vuelos de SOLO IDA
            for (int i = 0; i < 100; i++)
            {
                var ruta = rutasBase[i % rutasBase.Length];
                var dia = i / rutasBase.Length;
                var horaSalida = new TimeSpan(6 + (i % 16), random.Next(0, 60), 0); // Entre 6 AM y 10 PM
                var horaLlegada = horaSalida.Add(TimeSpan.FromMinutes(ruta.duracion));

                vuelos.Add(new Vuelo
                {
                    NumeroVuelo = $"RD{numeroVuelo++}",
                    Fecha = hoy.AddDays(dia),
                    HoraSalida = horaSalida,
                    HoraLlegada = horaLlegada,
                    Duracion = ruta.duracion,
                    PrecioBase = ruta.precio,
                    OrigenCodigo = ruta.origen,
                    DestinoCodigo = ruta.destino,
                    Matricula = matriculas[i % matriculas.Length],
                    Estado = "Programado",
                    TipoVuelo = "SoloIda",
                    FechaRegreso = null
                });
            }

            await context.Vuelos.AddRangeAsync(vuelos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAsientosAsync(AppDbContext context)
        {
            var vuelos = await context.Vuelos.ToListAsync();
            var asientos = new List<Asiento>();

            foreach (var vuelo in vuelos)
            {
                // Primera Clase (Filas 1-3): 12 asientos
                for (int fila = 1; fila <= 3; fila++)
                {
                    foreach (var letra in new[] { "A", "B", "C", "D" })
                    {
                        asientos.Add(new Asiento
                        {
                            Numero = $"{vuelo.Id}-{fila}{letra}",
                            IdVuelo = vuelo.Id,
                            Clase = "Primera",
                            Disponibilidad = "Disponible"
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
                            Numero = $"{vuelo.Id}-{fila}{letra}",
                            IdVuelo = vuelo.Id,
                            Clase = "Ejecutiva",
                            Disponibilidad = "Disponible"
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
                            Numero = $"{vuelo.Id}-{fila}{letra}",
                            IdVuelo = vuelo.Id,
                            Clase = "Economica",
                            Disponibilidad = "Disponible"
                        });
                    }
                }
            }

            await context.Asientos.AddRangeAsync(asientos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedClientesAsync(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            var clienteUser = await userManager.FindByEmailAsync("cliente@test.com");

            var clientes = new List<Cliente>
            {
                new Cliente
                {
                    Nombre = "Juan Cliente",
                    Email = "cliente@test.com",
                    Telefono = "+1 809-555-0001",
                    UserId = clienteUser?.Id
                },
                new Cliente
                {
                    Nombre = "María González",
                    Email = "maria.gonzalez@example.com",
                    Telefono = "+1 809-555-0002"
                },
                new Cliente
                {
                    Nombre = "Pedro Sánchez",
                    Email = "pedro.sanchez@example.com",
                    Telefono = "+1 809-555-0003"
                }
            };

            await context.Clientes.AddRangeAsync(clientes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPasajerosAsync(AppDbContext context)
        {
            var pasajeros = new List<Pasajero>
            {
                new Pasajero { Nombre = "Juan", Apellido = "Cliente", Pasaporte = "A12345678" },
                new Pasajero { Nombre = "María", Apellido = "González", Pasaporte = "B23456789" },
                new Pasajero { Nombre = "Pedro", Apellido = "Sánchez", Pasaporte = "C34567890" },
                new Pasajero { Nombre = "Ana", Apellido = "Martínez", Pasaporte = "D45678901" },
                new Pasajero { Nombre = "Luis", Apellido = "Rodríguez", Pasaporte = "E56789012" }
            };

            await context.Pasajeros.AddRangeAsync(pasajeros);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReservasAsync(AppDbContext context)
        {
            var vuelos = await context.Vuelos.Take(5).ToListAsync();
            var clientes = await context.Clientes.ToListAsync();
            var pasajeros = await context.Pasajeros.ToListAsync();
            var asientos = await context.Asientos
                .Where(a => a.Clase == "Economica" && a.Disponibilidad == "Disponible")
                .Take(5)
                .ToListAsync();

            if (!vuelos.Any() || !clientes.Any() || !pasajeros.Any() || !asientos.Any())
                return;

            var reservas = new List<Reserva>
            {
                new Reserva
                {
                    Codigo = "RES001",
                    IdVuelo = vuelos[0].Id,
                    IdCliente = clientes[0].Id,
                    IdPasajero = pasajeros[0].Id,
                    NumAsiento = asientos[0].Numero,
                    FechaReserva = DateTime.Today.AddDays(-5),
                    Estado = "Confirmada",
                    PrecioTotal = vuelos[0].PrecioBase
                },
                new Reserva
                {
                    Codigo = "RES002",
                    IdVuelo = vuelos[1].Id,
                    IdCliente = clientes[1].Id,
                    IdPasajero = pasajeros[1].Id,
                    NumAsiento = asientos[1].Numero,
                    FechaReserva = DateTime.Today.AddDays(-3),
                    Estado = "Confirmada",
                    PrecioTotal = vuelos[1].PrecioBase
                },
                new Reserva
                {
                    Codigo = "RES003",
                    IdVuelo = vuelos[2].Id,
                    IdCliente = clientes[2].Id,
                    IdPasajero = pasajeros[2].Id,
                    NumAsiento = asientos[2].Numero,
                    FechaReserva = DateTime.Today.AddDays(-1),
                    Estado = "Confirmada",
                    PrecioTotal = vuelos[2].PrecioBase
                }
            };

            await context.Reservas.AddRangeAsync(reservas);
            await context.SaveChangesAsync();

            // Marcar asientos como ocupados
            foreach (var asiento in asientos.Take(3))
            {
                asiento.Disponibilidad = "Ocupado";
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedFacturasAsync(AppDbContext context)
        {
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
        }

        private static async Task SeedVueloTripulacionAsync(AppDbContext context)
        {
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

            await context.VueloTripulaciones.AddRangeAsync(vueloTripulaciones);
            await context.SaveChangesAsync();
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
    }
}