using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using System.Globalization;
using System.Text;

namespace AerolineaRD.Services
{
    public class VueloService : IVueloService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IMapper _mapper;

        public VueloService(IVueloRepository vueloRepository, IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _mapper = mapper;
        }

        public async Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros)
        {
            var vuelos = await _vueloRepository.BuscarVuelosConFiltrosAsync(
                filtros.Origen,
                filtros.Destino,
                filtros.FechaSalidaInicio,       // fechaSalidaInicio (nullable)
                filtros.FechaSalidaFin,    // fechaSalidaFin (nullable)
                filtros.FechaRegresoInicio,      // fechaRegresoInicio (nullable)
                filtros.FechaRegresoFin,   // fechaRegresoFin (nullable)
                filtros.Clase,
                filtros.TipoViaje
            );

            // ✅ NO filtrar vuelos - TODOS los vuelos tienen las 3 clases disponibles
            // Solo calcular el precio según la clase solicitada
            var resultados = vuelos
  .Where(v => v.Aeronave?.Asientos != null && v.Aeronave.Asientos.Any())
                .Select(vuelo =>
{
    var vueloDto = _mapper.Map<VueloResponseDto>(vuelo);

    // ✅ Calcular disponibilidad de asientos por clase (TODAS las clases)
    var clasesDisponibles = CalcularClasesDisponibles(vuelo, null);

    // ✅ Si se especificó una clase, mostrar solo esa clase con su precio
    if (!string.IsNullOrEmpty(filtros.Clase))
    {
        // ✅ Normalizar la clase: "Primera Clase" -> "Primera", "Ejecutiva" -> "Ejecutiva"
        var claseNormalizada = filtros.Clase.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        
        var claseSeleccionada = clasesDisponibles
         .FirstOrDefault(c => string.Equals(c.Clase, claseNormalizada, StringComparison.OrdinalIgnoreCase));

        if (claseSeleccionada != null)
        {
            // Mostrar solo la clase solicitada
            vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto> { claseSeleccionada };
            // Actualizar el precio base del DTO para reflejar el precio de esta clase
            vueloDto.PrecioBase = claseSeleccionada.Precio;
        }
        else
          {
       // ✅ Si la clase no existe en la aeronave (caso raro), crear la entrada manualmente
   var precioClase = CalcularPrecioPorClase(vuelo.PrecioBase, claseNormalizada);
    vueloDto.ClasesDisponibles = new List<ClaseDisponibilidadDto>
        {
     new ClaseDisponibilidadDto
   {
         Clase = claseNormalizada,
      AsientosDisponibles = 0,
  Precio = precioClase
     }
    };
      vueloDto.PrecioBase = precioClase;
     }
   }
    else
    {
        // Sin filtro de clase - mostrar todas las clases disponibles
        vueloDto.ClasesDisponibles = clasesDisponibles;
    }

    return vueloDto;
})
           .Where(v => v.ClasesDisponibles.Any()) // Solo mostrar vuelos con clases definidas
                .ToList();

            return resultados;
        }

        public async Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);

            if (vuelo == null) return null;

            var vueloDto = _mapper.Map<VueloResponseDto>(vuelo);
            vueloDto.ClasesDisponibles = vuelo.Aeronave?.Asientos != null
       ? CalcularClasesDisponibles(vuelo, null)
          : new List<ClaseDisponibilidadDto>();

            return vueloDto;
        }

        public async Task<List<AsientoDisponibleDto>> ObtenerAsientosDisponiblesAsync(int idVuelo, string clase)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(idVuelo);

            if (vuelo?.Aeronave?.Asientos == null)
                return new List<AsientoDisponibleDto>();

            // Normalizar la clase para comparación
            var claseNormalizada = NormalizarClase(clase);

            // Obtener asientos ocupados
            var asientosOcupados = (vuelo.Reservas ?? Enumerable.Empty<Reserva>())
            .Where(r => r.Estado == "Confirmada" && !string.IsNullOrEmpty(r.NumAsiento))
                  .Select(r => r.NumAsiento!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Filtrar asientos por clase y mapear a DTO
            var asientos = vuelo.Aeronave.Asientos
       .Where(a => NormalizarClase(a.Clase ?? "Economica") == claseNormalizada)
             .OrderBy(a => a.NumeroAsiento)
        .Select(a => new AsientoDisponibleDto
        {
            Numero = a.NumeroAsiento ?? "",
            Clase = a.Clase ?? "Economica",
            Disponible = !asientosOcupados.Contains(a.NumeroAsiento ?? ""),
            Fila = ExtraerFila(a.NumeroAsiento ?? ""),
            Columna = ExtraerColumna(a.NumeroAsiento ?? "")
        })
          .ToList();

            return asientos;
        }

        private static int ExtraerFila(string numeroAsiento)
        {
            if (string.IsNullOrEmpty(numeroAsiento))
                return 0;

            var numeros = new string(numeroAsiento.Where(char.IsDigit).ToArray());
            return int.TryParse(numeros, out var fila) ? fila : 0;
        }

        private static string ExtraerColumna(string numeroAsiento)
        {
            if (string.IsNullOrEmpty(numeroAsiento))
                return "";

            return new string(numeroAsiento.Where(char.IsLetter).ToArray());
        }

        private List<ClaseDisponibilidadDto> CalcularClasesDisponibles(Vuelo vuelo, string? filtroClase)
        {
            if (vuelo.Aeronave?.Asientos == null || !vuelo.Aeronave.Asientos.Any())
         return new List<ClaseDisponibilidadDto>();

            // ✅ Obtener asientos ocupados
 var asientosOcupados = (vuelo.Reservas ?? Enumerable.Empty<Reserva>())
     .Where(r => r.Estado == "Confirmada" && !string.IsNullOrEmpty(r.NumAsiento))
     .Select(r => r.NumAsiento!)
     .ToHashSet(StringComparer.OrdinalIgnoreCase);

  // ✅ Agrupar por clase normalizada para evitar problemas con mayúsculas/minúsculas
        var asientosPorClase = vuelo.Aeronave.Asientos
 .GroupBy(a => (a.Clase ?? "Economica").ToUpperInvariant())
     .ToList();

      // ✅ Calcular TODAS las clases disponibles (Económica, Ejecutiva, Primera)
 var clasesCalculadas = asientosPorClase
          .Select(g => new ClaseDisponibilidadDto
      {
  Clase = g.First().Clase ?? "Economica",
   AsientosDisponibles = g.Count(a => !asientosOcupados.Contains(a.NumeroAsiento ?? "")),
   Precio = CalcularPrecioPorClase(vuelo.PrecioBase, g.Key)
      })
     .ToList();

     // ✅ Si se especifica un filtro de clase, devolver solo esa clase
 if (!string.IsNullOrEmpty(filtroClase))
    {
      var filtroNormalizado = filtroClase.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].ToUpperInvariant();
   clasesCalculadas = clasesCalculadas
     .Where(c => c.Clase.ToUpperInvariant() == filtroNormalizado)
    .ToList();
   }

     return clasesCalculadas;
        }

        private static decimal CalcularPrecioPorClase(decimal precioBase, string clase)
        {
    // ✅ Normalizar la clase para comparación case-insensitive
    var claseNormalizada = clase.ToUpperInvariant();
            
    return claseNormalizada switch
       {
   "PRIMERA" => precioBase + 200m,
     "EJECUTIVA" => precioBase + 100m,
       "ECONOMICA" => precioBase,
     _ => precioBase
        };
        }

        private static string NormalizarClase(string? clase)
        {
            if (string.IsNullOrEmpty(clase))
                return string.Empty;

            var primeraPalabra = clase.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            return NormalizarTexto(primeraPalabra);
        }

        private static string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in textoNormalizado)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString()
       .Normalize(NormalizationForm.FormC)
           .ToLowerInvariant();
        }
    }
}