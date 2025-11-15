
using AerolineaRD.Entity;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Asegurar que la base de datos existe
            await context.Database.EnsureCreatedAsync();

            // Si ya hay datos, no hacer nada
            if (await context.Aeropuertos.AnyAsync())
            {
                return;
            }

            // 1. AEROPUERTOS
            var aeropuertos = new List<Aeropuerto>
            {
                // República Dominicana
                new Aeropuerto
                {
                    Codigo = "SDQ",
                    Nombre = "Aeropuerto Internacional Las Américas",
                    Ciudad = "Santo Domingo",
                    Pais = "República Dominicana"
                },
                new Aeropuerto
                {
                    Codigo = "PUJ",
                    Nombre = "Aeropuerto Internacional Punta Cana",
                    Ciudad = "Punta Cana",
                    Pais = "República Dominicana"
                },
                new Aeropuerto
                {
                    Codigo = "STI",
                    Nombre = "Aeropuerto Internacional Cibao",
                    Ciudad = "Santiago",
                    Pais = "República Dominicana"
                },
                new Aeropuerto
                {
                    Codigo = "POP",
                    Nombre = "Aeropuerto Internacional Gregorio Luperón",
                    Ciudad = "Puerto Plata",
                    Pais = "República Dominicana"
                },
                new Aeropuerto
                {
                    Codigo = "LRM",
                    Nombre = "Aeropuerto Internacional La Romana",
                    Ciudad = "La Romana",
                    Pais = "República Dominicana"
                },
                // Estados Unidos
                new Aeropuerto
                {
                    Codigo = "JFK",
                    Nombre = "John F. Kennedy International Airport",
                    Ciudad = "New York",
                    Pais = "Estados Unidos"
                },
                new Aeropuerto
                {
                    Codigo = "MIA",
                    Nombre = "Miami International Airport",
                    Ciudad = "Miami",
                    Pais = "Estados Unidos"
                },
                new Aeropuerto
                {
                    Codigo = "FLL",
                    Nombre = "Fort Lauderdale-Hollywood International",
                    Ciudad = "Fort Lauderdale",
                    Pais = "Estados Unidos"
                },
                new Aeropuerto
                {
                    Codigo = "EWR",
                    Nombre = "Newark Liberty International Airport",
                    Ciudad = "Newark",
                    Pais = "Estados Unidos"
                },
                new Aeropuerto
                {
                    Codigo = "ATL",
                    Nombre = "Hartsfield-Jackson Atlanta International",
                    Ciudad = "Atlanta",
                    Pais = "Estados Unidos"
                },
                // Europa
                new Aeropuerto
                {
                    Codigo = "MAD",
                    Nombre = "Aeropuerto Adolfo Suárez Madrid-Barajas",
                    Ciudad = "Madrid",
                    Pais = "España"
                },
                new Aeropuerto
                {
                    Codigo = "BCN",
                    Nombre = "Aeropuerto de Barcelona-El Prat",
                    Ciudad = "Barcelona",
                    Pais = "España"
                },
                new Aeropuerto
                {
                    Codigo = "CDG",
                    Nombre = "Aéroport Paris-Charles de Gaulle",
                    Ciudad = "París",
                    Pais = "Francia"
                },
                // Latinoamérica
                new Aeropuerto
                {
                    Codigo = "PTY",
                    Nombre = "Aeropuerto Internacional de Tocumen",
                    Ciudad = "Ciudad de Panamá",
                    Pais = "Panamá"
                },
                new Aeropuerto
                {
                    Codigo = "BOG",
                    Nombre = "Aeropuerto Internacional El Dorado",
                    Ciudad = "Bogotá",
                    Pais = "Colombia"
                },
                new Aeropuerto
                {
                    Codigo = "CCS",
                    Nombre = "Aeropuerto Internacional Simón Bolívar",
                    Ciudad = "Caracas",
                    Pais = "Venezuela"
                },
                new Aeropuerto
                {
                    Codigo = "CUN",
                    Nombre = "Aeropuerto Internacional de Cancún",
                    Ciudad = "Cancún",
                    Pais = "México"
                },
                new Aeropuerto
                {
                    Codigo = "HAV",
                    Nombre = "Aeropuerto Internacional José Martí",
                    Ciudad = "La Habana",
                    Pais = "Cuba"
                }
            };

            await context.Aeropuertos.AddRangeAsync(aeropuertos);
            await context.SaveChangesAsync();

            // 2. VUELOS (próximos 30 días)
            var vuelos = new List<Vuelo>();
            var random = new Random();
            var fechaBase = DateTime.Now.Date;

            // Rutas populares desde RD
            var rutas = new List<(string origen, string destino, string numeroBase)>
            {
                // Desde Santo Domingo
                ("SDQ", "JFK", "AR100"),
                ("SDQ", "MIA", "AR200"),
                ("SDQ", "FLL", "AR300"),
                ("SDQ", "ATL", "AR400"),
                ("SDQ", "MAD", "AR500"),
                ("SDQ", "PTY", "AR600"),
                ("SDQ", "BOG", "AR700"),
                
                // Desde Punta Cana
                ("PUJ", "JFK", "AR110"),
                ("PUJ", "MIA", "AR210"),
                ("PUJ", "EWR", "AR310"),
                ("PUJ", "CDG", "AR410"),
                ("PUJ", "MAD", "AR510"),
                
                // Desde Santiago
                ("STI", "MIA", "AR220"),
                ("STI", "JFK", "AR120"),
                
                // Desde Puerto Plata
                ("POP", "MIA", "AR230"),
                ("POP", "CUN", "AR330"),
                
                // Vuelos regionales
                ("SDQ", "CUN", "AR800"),
                ("SDQ", "HAV", "AR900"),
                ("PUJ", "CCS", "AR610"),
                
                // Vuelos de regreso (ida y vuelta)
                ("JFK", "SDQ", "AR101"),
                ("MIA", "SDQ", "AR201"),
                ("FLL", "SDQ", "AR301"),
                ("ATL", "SDQ", "AR401"),
                ("MAD", "SDQ", "AR501"),
                ("JFK", "PUJ", "AR111"),
                ("MIA", "PUJ", "AR211"),
                ("PTY", "SDQ", "AR601"),
                ("BOG", "SDQ", "AR701")
            };

            int vueloId = 1;
            foreach (var (origen, destino, numeroBase) in rutas)
            {
                // Crear vuelos para los próximos 30 días (algunos días sí, otros no)
                for (int dia = 0; dia < 30; dia++)
                {
                    // No todos los vuelos van todos los días
                    if (random.Next(100) < 70) // 70% de probabilidad
                    {
                        var fecha = fechaBase.AddDays(dia);
                        vuelos.Add(new Vuelo
                        {
                            NumeroVuelo = $"{numeroBase}{(dia % 10)}",
                            Fecha = fecha,
                            OrigenCodigo = origen,
                            DestinoCodigo = destino
                        });
                    }
                }
            }

            await context.Vuelos.AddRangeAsync(vuelos);
            await context.SaveChangesAsync();

            // 3. ASIENTOS (para cada vuelo)
            var asientos = new List<Asiento>();
            var clases = new[] { "Economica", "Ejecutiva", "Primera" };
            var disponibilidades = new[] { "Disponible", "Ocupado", "Reservado" };

            foreach (var vuelo in vuelos)
            {
                // Primera Clase: 4 asientos (1A-1D)
                for (int i = 0; i < 4; i++)
                {
                    char letra = (char)('A' + i);
                    asientos.Add(new Asiento
                    {
                        Numero = $"1{letra}-{vuelo.Id}",
                        IdVuelo = vuelo.Id,
                        Clase = "Primera",
                        Disponibilidad = random.Next(100) < 60 ? "Disponible" : disponibilidades[random.Next(disponibilidades.Length)]
                    });
                }

                // Clase Ejecutiva: 12 asientos (2A-4F)
                for (int fila = 2; fila <= 4; fila++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        char letra = (char)('A' + i);
                        asientos.Add(new Asiento
                        {
                            Numero = $"{fila}{letra}-{vuelo.Id}",
                            IdVuelo = vuelo.Id,
                            Clase = "Ejecutiva",
                            Disponibilidad = random.Next(100) < 70 ? "Disponible" : disponibilidades[random.Next(disponibilidades.Length)]
                        });
                    }
                }

                // Clase Económica: 144 asientos (5A-28F)
                for (int fila = 5; fila <= 28; fila++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        char letra = (char)('A' + i);
                        asientos.Add(new Asiento
                        {
                            Numero = $"{fila}{letra}-{vuelo.Id}",
                            IdVuelo = vuelo.Id,
                            Clase = "Economica",
                            Disponibilidad = random.Next(100) < 75 ? "Disponible" : disponibilidades[random.Next(disponibilidades.Length)]
                        });
                    }
                }
            }

            await context.Asientos.AddRangeAsync(asientos);
            await context.SaveChangesAsync();

            Console.WriteLine($"? Seeder completado:");
            Console.WriteLine($"   - {aeropuertos.Count} aeropuertos");
            Console.WriteLine($"   - {vuelos.Count} vuelos");
            Console.WriteLine($"   - {asientos.Count} asientos");
        }
    }
}