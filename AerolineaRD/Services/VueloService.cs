using AerolineaRD.Data;
using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Services
{
    public class VueloService : IVueloService
    {
        private readonly AppDbContext _context;

        public VueloService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros)
        {
            var query = _context.Vuelos
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Asientos)
                .AsQueryable();

            // Filtro por origen
            if (!string.IsNullOrEmpty(filtros.Origen))
            {
                query = query.Where(v => v.OrigenCodigo == filtros.Origen);
            }

            // Filtro por destino
            if (!string.IsNullOrEmpty(filtros.Destino))
            {
                query = query.Where(v => v.DestinoCodigo == filtros.Destino);
            }

            // Filtro por fecha de salida
            if (filtros.FechaSalida.HasValue)
            {
                var fecha = filtros.FechaSalida.Value.Date;
                query = query.Where(v => v.Fecha.Date == fecha);
            }

            // CAMBIO: Traer los datos a memoria primero
            var vuelosEnMemoria = await query
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            // Filtrar por clase en memoria (después de cargar desde BD)
            if (!string.IsNullOrEmpty(filtros.Clase))
            {
                vuelosEnMemoria = vuelosEnMemoria
                    .Where(v => v.Asientos.Any(a => a.Clase == filtros.Clase && a.Disponibilidad == "Disponible"))
                    .ToList();
            }

            // Proyectar a DTO
            var vuelos = vuelosEnMemoria
                .Select(v => new VueloResponseDto
                {
                    Id = v.Id,
                    NumeroVuelo = v.NumeroVuelo,
                    Fecha = v.Fecha,
                    OrigenCodigo = v.OrigenCodigo,
                    OrigenNombre = v.Origen?.Nombre,
                    OrigenCiudad = v.Origen?.Ciudad,
                    DestinoCodigo = v.DestinoCodigo,
                    DestinoNombre = v.Destino?.Nombre,
                    DestinoCiudad = v.Destino?.Ciudad,
                    AsientosDisponibles = v.Asientos.Count(a => a.Disponibilidad == "Disponible"),
                    ClasesDisponibles = v.Asientos
                        .Where(a => a.Disponibilidad == "Disponible")
                        .Select(a => a.Clase)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            return vuelos;
        }

        public async Task<List<AeropuertoDto>> ObtenerAeropuertosAsync()
        {
            return await _context.Aeropuertos
                .Select(a => new AeropuertoDto
                {
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Ciudad = a.Ciudad,
                    Pais = a.Pais
                })
                .OrderBy(a => a.Ciudad)
                .ToListAsync();
        }

        public async Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id)
        {
            var vuelo = await _context.Vuelos
                .Include(v => v.Origen)
                .Include(v => v.Destino)
                .Include(v => v.Asientos)
                .Where(v => v.Id == id)
                .FirstOrDefaultAsync();

            if (vuelo == null)
                return null;

            return new VueloResponseDto
            {
                Id = vuelo.Id,
                NumeroVuelo = vuelo.NumeroVuelo,
                Fecha = vuelo.Fecha,
                OrigenCodigo = vuelo.OrigenCodigo,
                OrigenNombre = vuelo.Origen?.Nombre,
                OrigenCiudad = vuelo.Origen?.Ciudad,
                DestinoCodigo = vuelo.DestinoCodigo,
                DestinoNombre = vuelo.Destino?.Nombre,
                DestinoCiudad = vuelo.Destino?.Ciudad,
                AsientosDisponibles = vuelo.Asientos.Count(a => a.Disponibilidad == "Disponible"),
                ClasesDisponibles = vuelo.Asientos
                    .Where(a => a.Disponibilidad == "Disponible")
                    .Select(a => a.Clase)
                    .Distinct()
                    .ToList()
            };
        }
    }
}